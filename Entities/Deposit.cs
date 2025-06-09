namespace MiniBank.Entities
{
    [ValidatorAttribute(typeof(Validators.DepositValidator))]
    public class Deposit : IDatabaseEntity
    {
        public long Id { get; set; }
        public long AccountId { get; init; }
        public decimal Amount { get; init; }

        public Deposit(Account account, decimal amount)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account), "Account cannot be null.");

            AccountId = account.Id;
            Amount = amount;
        }

    }
}
