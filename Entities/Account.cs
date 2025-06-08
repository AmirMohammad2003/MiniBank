using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Entities
{
    public class Account: IDatabaseEntity
    {
        public long Id { get; set; }
        public long AccountNumber { get; set; }
        public decimal Balance { get; set; } = 0.0m;
        public bool IsActive { get; set; } = true;
        public long CardId { get; set; }
        public long UserId { get; set; }

        public Account(User user, long cardId)
        {
            CardId = cardId;
            AccountNumber = GenerateAccountNumber();
            UserId = user.Id;
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
    
    }
}
