namespace MiniBank.Entities
{
    public class Account : IDatabaseEntity
    {
        public long Id { get; set; }
        public long AccountNumber { get; init; }
        public long CardId { get; init; }
        public long UserId { get; init; }

        public bool IsActive { get; set; }

        private decimal _balance;

        public decimal Balance
        {
            get => _balance;
        }

        public Account(User user, long cardId)
        {
            CardId = cardId;
            UserId = user.Id;
            AccountNumber = GenerateAccountNumber();
            _balance = 0.0m;
            IsActive = true;
        }

        private static long GenerateAccountNumber()
        {
            Random random = new();
            long accountNumber;
            do
            {
                accountNumber = random.Next(100000000, 999999999);
            } while (Database.Instance.Exists<Account>(a => a?.AccountNumber == accountNumber));
            return accountNumber;
        }


        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

            _balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));

            if (amount > _balance)
                throw new InvalidOperationException("Insufficient funds for withdrawal.");

            _balance -= amount;
        }

    }
}
