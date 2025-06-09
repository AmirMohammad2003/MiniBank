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

        private const int ExpirationTime = 1;

        public PassCode(string code, long accountId, long recivingAccountId, decimal amount)
        {
            AccountId = accountId;
            RecivingAccountId = recivingAccountId;
            Amount = amount;
            Code = code;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.AddMinutes(ExpirationTime);
        }

        public bool IsValid()
        {
            return DateTime.UtcNow < ExpiresAt;
        }
    }
}
