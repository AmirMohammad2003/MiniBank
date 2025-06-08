using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Entities
{
    public class Transaction: IDatabaseEntity
    {
        public long Id { get; set; }
        public long FromAccountId { get; set; }
        public long ToAccountId { get; set; }
        public decimal Amount { get; set; } = 0.0m;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;

    }
}
