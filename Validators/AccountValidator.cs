using MiniBank.Entities;
namespace MiniBank.Validators
{

    public class AccountValidator : IValidator<Account>
    {
        public void Validate(Account entity)
        {
            if (entity == null)
            {
                throw new ValidationError("Account cannot be null.");
            }

            // not much to check here
        }
    }
}
