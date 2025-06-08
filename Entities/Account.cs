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
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0.0m;
        public bool IsActive { get; set; } = true;
        public Card AccountCard { get; set; } = new Card();

        public class Card
        {
            public string CardNumber { get; set; } = string.Empty;
            public int Cvv2 { get; set; } = 0;
            public int Passcode { get; set; } = 0;
            public int Passcode2 { get; set; } = 0;
            public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(5);
        }
    
    }
}
