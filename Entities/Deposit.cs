using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Entities
{
    [ValidatorAttribute(typeof(Validators.DepositValidator))]
    public class Deposit: IDatabaseEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public decimal Amount { get; set; } = 0.0m;

        public Deposit(Account account, decimal amount)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account), "Account cannot be null.");

            AccountId = account.Id;
            Amount = amount;
        }

    }
}
