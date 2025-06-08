using MiniBank.Entities;

namespace MiniBank
{
    internal class Program
    {
        static void Main()
        {
            static void notifyEntity(IDatabaseEntity entity)
            {
                Console.WriteLine($"Entity changed: {entity.GetType().Name} with ID {entity.Id}");
                foreach (var prop in entity.GetType().GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(entity)}");
                }
            }

            static void notifySignin(Entities.User user)
            {
                Console.WriteLine($"User signed in: {user.UserName} with ID {user.Id}");
            }

            Database.Instance.OnEntitySaved += notifyEntity;
            Database.Instance.OnEntityUpdated += notifyEntity;
            Services.AccountServices.OnSignIn += notifySignin;
            Services.AccountServices.CreateAccount(
                "heshmat_sr",
                "securepassword",
                "Heshmat",
                "Sr.",
                "1234567890",
                "09123456789",
                1000.00m
            );
            Services.AccountServices.CreateAccount(
                "heshmat_jr",
                "securepassword",
                "Heshmat",
                "Jr.",
                "1234567891",
                "09223456789",
                1000.00m
            );
            Database.Instance.FetchAll<Account>().ToList().ForEach(account =>
            {
                Console.WriteLine($"Account ID: {account.Id}, Account Number: {account.AccountNumber}, Balance: {account.Balance}");
            });
            
            var userId = Services.AccountServices.SignIn("heshmat_sr", "securepassword");
            if (Services.AccountServices.CheckPayment(1, 2, 101.00m))
            {
                var input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                {
                    if (Services.AccountServices.ContinuePayment(1, 2, 101.00m, result))
                        Console.WriteLine("Payment successful.");

                }
            }
            Database.Instance.OnEntitySaved -= notifyEntity;
            Database.Instance.OnEntityUpdated -= notifyEntity;
            Services.AccountServices.OnSignIn -= notifySignin;
        }
    }
}
