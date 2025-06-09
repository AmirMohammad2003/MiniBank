namespace MiniBank
{
    public interface IValidator<TEntity> where TEntity : IDatabaseEntity
    {
        void Validate(TEntity entity);
    }
}
