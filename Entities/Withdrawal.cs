using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Entities
{
    public class Withdrawal : IDatabaseEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public decimal Amount { get; set; } = 0.0m;

    }
}
