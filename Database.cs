namespace MiniBank
{
    internal class Database
    {
        private static Database? _instance;
        public static Database Instance
        {
            get
            {
                _instance ??= new Database();
                return _instance;
            }
        }

        private readonly Dictionary<Type, List<IDatabaseEntity>> _dataStore;

        private readonly Dictionary<Type, long> _nextId;

        public event Action<IDatabaseEntity>? OnEntitySaved;

        public event Action<IDatabaseEntity>? OnEntityUpdated;

        public event Action<IDatabaseEntity>? OnEntityDeleted;

        private static bool Validate<TDatabaseEntity>(TDatabaseEntity entity, bool raiseException = true) where TDatabaseEntity : IDatabaseEntity
        {
            Type type = typeof(TDatabaseEntity);
            if (!type.IsDefined(typeof(ValidatorAttribute), false))
            {
                return true;
            }

            ValidatorAttribute[] attributes = (ValidatorAttribute[])type.GetCustomAttributes(typeof(ValidatorAttribute), false);
            foreach (var attribute in attributes)
            {
                IValidator<TDatabaseEntity> validator = (IValidator<TDatabaseEntity>)Activator.CreateInstance(attribute.ValidatorType)!;
                if (raiseException)
                {
                    validator.Validate(entity);
                }
                else
                {
                    try
                    {
                        validator.Validate(entity);
                    }
                    catch (ValidationError)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        private List<IDatabaseEntity> GetList(Type type)
        {
            if (!_dataStore.TryGetValue(type, out List<IDatabaseEntity>? value))
            {
                value = [];
                _dataStore[type] = value;
            }
            return value;
        }

        public Database()
        {
            _dataStore = [];
            _nextId = [];
        }

        public TDatabaseEntity? Get<TDatabaseEntity>(long id) where TDatabaseEntity : IDatabaseEntity
        {
            IEnumerable<IDatabaseEntity> res = GetList(typeof(TDatabaseEntity)).Where(entity => entity.Id == id);
            if (res.Count() > 1) throw new InvalidOperationException("More than one entity with the same ID found.");
            IDatabaseEntity? fetched = res.FirstOrDefault();
            if (fetched == null)
            {
                return default;
            }
            return (TDatabaseEntity)fetched;
        }

        public void Save<TDatabaseEntity>(TDatabaseEntity entity, bool signal = true) where TDatabaseEntity : IDatabaseEntity
        {
            Type type = typeof(TDatabaseEntity);
            if (!_nextId.ContainsKey(type))
            {
                _nextId[type] = 0L;
            }

            if (entity.Id == 0L)
            {
                entity.Id = ++_nextId[type];
            }
            else if (entity.Id < 0L)
            {
                throw new InvalidOperationException("ID must be a positive number or zero for new entities.");
            }
            else if (Get<TDatabaseEntity>(entity.Id) != null)
            {
                throw new InvalidOperationException($"Entity with the same ID already exists.");
            }
            else
            {
                _nextId[type] = Math.Max(_nextId[type], entity.Id);
            }

            Validate(entity, true);
            GetList(typeof(TDatabaseEntity)).Add(entity);
            if (signal)
            {
                OnEntitySaved?.Invoke(entity);
            }
        }

        public void Update<TDatabaseEntity>(TDatabaseEntity entity, bool signal = true) where TDatabaseEntity : IDatabaseEntity
        {
            TDatabaseEntity? old_entity = Get<TDatabaseEntity>(entity.Id);
            if (entity == null)
            {
                throw new InvalidOperationException("Entity not found for update.");
            }
            Validate(entity, true);
            Delete(old_entity!, false);
            Save(entity, false);
            if (signal)
            {
                OnEntityUpdated?.Invoke(entity);
            }
        }

        public void Delete<TDatabaseEntity>(TDatabaseEntity entity, bool signal = true) where TDatabaseEntity : IDatabaseEntity
        {
            GetList(typeof(TDatabaseEntity)).Remove(entity);
            if (signal)
            {
                OnEntityDeleted?.Invoke(entity);
            }
        }

        public IEnumerable<TDatabaseEntity> FetchAll<TDatabaseEntity>() where TDatabaseEntity : IDatabaseEntity
        {
            return GetList(typeof(TDatabaseEntity)).Cast<TDatabaseEntity>();
        }

        public bool Exists<TDatabaseEntity>(Func<TDatabaseEntity?, bool> evaluate) where TDatabaseEntity : IDatabaseEntity
        {
            return GetList(typeof(TDatabaseEntity)).Any(entity => evaluate((TDatabaseEntity)entity));
        }

        public IEnumerable<TDatabaseEntity> Filter<TDatabaseEntity>(Func<TDatabaseEntity, bool> predicate) where TDatabaseEntity : IDatabaseEntity
        {
            return GetList(typeof(TDatabaseEntity)).Cast<TDatabaseEntity>().Where(predicate);
        }

        public T Reduce<TDatabaseEntity, T>(Func<TDatabaseEntity, T> selector, Func<T, T, T> reducer, T initialValue) where TDatabaseEntity : IDatabaseEntity
        {
            return GetList(typeof(TDatabaseEntity)).Cast<TDatabaseEntity>().Select(selector).Aggregate(initialValue, reducer);
        }
    }
}
