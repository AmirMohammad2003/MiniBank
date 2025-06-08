using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Entities
{
    public class DebitCard: IDatabaseEntity
    {
        public long Id { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public int Cvv2 { get; set; } = 0;
        public int Passcode { get; set; } = 0;
        public int Passcode2 { get; set; } = 0;
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(5);

        public DebitCard()
        {
            CardNumber = GenerateCardNumber();
            Cvv2 = GenerateCvv2();
            Passcode = GeneratePasscode();
            Passcode2 = GeneratePasscode();
        }

        private static int GenerateCvv2()
        {
            Random random = new();
            return random.Next(100, 1000);
        }

        private static string GenerateCardNumber()
        {
            Random random = new();
            string sample;
            do
            {
                sample = string.Join("", Enumerable.Range(0, 16).Select(_ => random.Next(0, 10).ToString()));
            } while (Database.Instance.Exists<DebitCard>(c => c?.CardNumber == sample));
            return sample;

        }

        private static int GeneratePasscode()
        {
            Random random = new();
            return random.Next(1000, 10000);
        }
    }
}
