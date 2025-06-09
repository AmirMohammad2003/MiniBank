using MiniBank.Entities;

namespace MiniBank.Validators
{
    public class WithdrawValidator : IValidator<Withdrawal>
    {
        public void Validate(Withdrawal entity)
        {
            if (entity == null)
            {
                throw new ValidationError("Deposit cannot be null.");
            }

            if (entity.Amount <= 0)
            {
                throw new ValidationError("Account ID must be greater than zero.");
            }
        }
    }
}
