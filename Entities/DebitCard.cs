namespace MiniBank.Entities
{
    public class DebitCard : IDatabaseEntity
    {
        public long Id { get; set; }
        public string CardNumber { get; init; }
        public int Cvv2 { get; init; }
        public DateTime ExpiryDate { get; init; } = DateTime.UtcNow.AddYears(5);

        public int Passcode { get; set; }
        public int Passcode2 { get; set; }

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
