using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        private long _nextId = 1;

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
        }

        TDatabaseEntity? Get<TDatabaseEntity>(long id) where TDatabaseEntity : IDatabaseEntity
        {
            IEnumerable<IDatabaseEntity> res = GetList(typeof(TDatabaseEntity)).Where(entity => entity.Id == id);
            if (res.Count() > 1) throw new InvalidOperationException("More than one entity with the same ID found.");
            IDatabaseEntity? fetched = res.FirstOrDefault();
            if(fetched == null)
            {
                return default;
            }
            return (TDatabaseEntity) fetched;
        }


        void Save<TDatabaseEntity>(TDatabaseEntity entity) where TDatabaseEntity : IDatabaseEntity
        {
            if (entity.Id == 0L)
            {
                entity.Id = _nextId++;
            }
            else if (entity.Id < 0L)
            {
                throw new InvalidOperationException("ID must be a positive number or zero for new entities.");
            }
            else if (Get<TDatabaseEntity>(entity.Id) != null)
            {
                throw new InvalidOperationException("Entity with the same ID already exists.");
            } 
            else
            {
                _nextId = Math.Max(_nextId, entity.Id + 1);
            }
            Validate(entity, true);
            GetList(typeof(TDatabaseEntity)).Add(entity);
            OnEntitySaved?.Invoke(entity);
        }

        void Update<TDatabaseEntity>(TDatabaseEntity entity) where TDatabaseEntity : IDatabaseEntity
        {
            TDatabaseEntity? old_entity = Get<TDatabaseEntity>(entity.Id);
            if (entity == null)
            {
                throw new InvalidOperationException("Entity not found for update.");
            }
            Validate(entity, true);
            Delete(old_entity!);
            Save(entity);
            OnEntityUpdated?.Invoke(entity);
        }

        void Delete<TDatabaseEntity>(TDatabaseEntity entity) where TDatabaseEntity : IDatabaseEntity
        {
            GetList(typeof(TDatabaseEntity)).Remove(entity);
            OnEntityDeleted?.Invoke(entity);
        }

        IEnumerable<TDatabaseEntity> FetchAll<TDatabaseEntity>() where TDatabaseEntity : IDatabaseEntity
        {
            return GetList(typeof(TDatabaseEntity)).Cast<TDatabaseEntity>().ToList();
        }
    }
}
