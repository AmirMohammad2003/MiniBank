namespace MiniBank.Entities
{
    [ValidatorAttribute(typeof(Validators.TransactionValidator))]
    public class Transaction : IDatabaseEntity
    {
        public long Id { get; set; }
        public long FromAccountId { get; init; }
        public long ToAccountId { get; init; }
        public decimal Amount { get; init; }
        public DateTime TransactionDate { get; init; } = DateTime.UtcNow;
        public string? Description { get; init; }
    }
}
