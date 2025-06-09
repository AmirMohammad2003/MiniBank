namespace MiniBank.Entities
{
    public class PassCode : IDatabaseEntity
    {
        public long Id { get; set; }
        public long AccountId { get; init; }
        public long RecivingAccountId { get; init; }
        public decimal Amount { get; init; }
        public string Code { get; init; }
        public DateTime CreatedAt { get; init; }
        private DateTime ExpiresAt;

        public PassCode(string code, long accountId, long recivingAccountId, decimal amount)
        {
            AccountId = accountId;
            RecivingAccountId = recivingAccountId;
            Amount = amount;
            Code = code;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.AddMinutes(1);
        }

        public bool IsValid()
        {
            return DateTime.UtcNow < ExpiresAt;
        }
    }
}
