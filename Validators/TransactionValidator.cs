using MiniBank.Entities;

namespace MiniBank.Validators
{
    public class TransactionValidator : IValidator<Transaction>
    {
        public void Validate(Transaction entity)
        {
            if (entity == null)
            {
                throw new ValidationError("Transaction cannot be null.");
            }

            if (entity.Amount <= 0)
            {
                throw new ValidationError("Amount must be greater than zero.");
            }

            Account? account = Database.Instance.Get<Account>(entity.FromAccountId);
            if (account == null || !Database.Instance.Exists<Account>(acc => acc?.Id == entity.ToAccountId))
            {
                throw new ValidationError($"Either the sending or reciving account does not exist.");
            }

            //  balance check isn't necessary here
        }
    }
}
