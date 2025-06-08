namespace MiniBank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Database.Instance.OnEntitySaved += entity =>
            {
                Console.WriteLine($"Entity saved: {entity.GetType().Name} with ID {entity.Id}");
                foreach (var prop in entity.GetType().GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(entity)}");
                }
            };

            Services.AccountServices.CreateAccount(
                "john_doe",
                "securepasswordal;sdkjf",
                "John",
                "Doe",
                "1234567890",
                "09123456789",
                1000.00m
            );
        }
    }
}
