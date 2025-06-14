﻿namespace MiniBank.Entities
{
    [ValidatorAttribute(typeof(Validators.WithdrawValidator))]
    public class Withdrawal : IDatabaseEntity
    {
        public long Id { get; set; }
        public long AccountId { get; init; }
        public decimal Amount { get; init; }
    }
}
