using MiniBank.Entities;
using System.ComponentModel.Design;

namespace MiniBank.CLI
{
    internal class SimpleCLI
    {
        static void Setup()
        {
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
        }

        static long SignIn()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            try
            {
                long userId = Services.AccountServices.SignIn(username, password);
                return userId;
            }
            catch (ArgumentException)
            {
                return -1;
            }
        }

        static void HandleSendingMoney(string[] parts, bool sendingToCard, Account account)
        {
            decimal sendAmount;
            if (parts.Length < 3 || parts.Length > 4 || !decimal.TryParse(parts[2], out sendAmount) || sendAmount <= 0)
            {
                Console.WriteLine("Usage: sendTo[Card] <accountNumber|cardNumber> <amount> [description]");
                return;
            }
            Entities.Account recivingAccount;
            if (sendingToCard)
            {
                var card = Database.Instance.Filter<Entities.DebitCard>(a => a.CardNumber.Trim().Equals(parts[1]?.Trim())).FirstOrDefault();
                recivingAccount = Database.Instance.Get<Entities.Account>((long)card.AccountId);
            }
            else
            {
                long accountNumber;
                if (!long.TryParse(parts[1], out accountNumber))
                {
                    Console.WriteLine("Invalid account number format. Please enter a valid account number.");
                    return;
                }
                recivingAccount = Database.Instance.Filter<Entities.Account>(a => a.AccountNumber == accountNumber).FirstOrDefault();
            }

            if (recivingAccount == null)
            {
                Console.WriteLine("Receiving account not found.");
                return;
            }
            if (recivingAccount.Id == account.Id)
            {
                Console.WriteLine("You cannot send money to your own account.");
                return;
            }
            if (Services.AccountServices.CheckPayment(account.Id, recivingAccount.Id, sendAmount))
            {
                Console.Write("Passcode: ");
                var code = Console.ReadLine();

                if (int.TryParse(code, out int result))
                {
                    if (Services.AccountServices.ContinuePayment(account.Id, recivingAccount.Id, sendAmount, result))
                    {
                        Console.WriteLine("Payment successful.");
                    }
                    else
                    {
                        Console.WriteLine("Payment failed. Incorrect passcode or insufficient funds.");
                    }

                }
            }
        }

        static void HandleInput(string input, Entities.User user)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var account = Database.Instance.Filter<Entities.Account>(acc => acc.UserId == user.Id).FirstOrDefault();
            if (account == null)
            {
                Console.WriteLine("No account found for the user.");
                return;
            }

            string command = parts[0].ToLowerInvariant();
            switch (command)
            {
                case "balance":
                    try
                    {
                        decimal balance = Services.AccountServices.AccountBalance(user.Id);
                        Console.WriteLine($"Account balance : {balance:C}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;

                case "deposit":
                    if (parts.Length < 2 || !decimal.TryParse(parts[1], out decimal depositAmount) || depositAmount <= 0)
                    {
                        Console.WriteLine("Usage: deposit <amount>");
                        break;
                    }
                    try
                    {
                        Services.AccountServices.Deposit(account!.Id, depositAmount);
                        Console.WriteLine($"Deposited {depositAmount:C} to account.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                    
                case "withdraw":
                    if (parts.Length < 2 || !decimal.TryParse(parts[1], out decimal withdrawAmount) || withdrawAmount <= 0)
                    {
                        Console.WriteLine("Usage: withdraw <amount>");
                        break;
                    }
                    try
                    {
                        Services.AccountServices.Withdraw(account!.Id, withdrawAmount);
                        Console.WriteLine($"Withdrew {withdrawAmount:C} from account.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;

                case "transactions":
                    var transactions = Services.AccountServices.GetTransactions(account!.Id);
                    transactions.ForEach(t =>
                    {
                        var fromAccount = Database.Instance.Get<Entities.Account>(t.FromAccountId);
                        var toAccount = Database.Instance.Get<Entities.Account>(t.ToAccountId);

                        Console.WriteLine($"Transaction ID: {t.Id}, Amount: {t.Amount:C},  Date: {t.TransactionDate}, From: {fromAccount?.AccountNumber}, To: {toAccount?.AccountNumber}");
                    });
                    break;

                case "sendto":
                    HandleSendingMoney(parts, false, account);
                    break;

                case "sendtocard":
                    HandleSendingMoney(parts, true, account);
                    break;

                case "quit":
                    Environment.Exit(0);
                    break;

                case "help":
                    Console.WriteLine("commands: balance, quit, help, deposit, withdraw, sendTo, sendToCard, transactions");
                    break;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }



        static void Main()
        {
            Setup();
            do
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "quit":
                        Environment.Exit(0);
                        break;
                    case "signin":
                        PrintAccounts();
                        long userId = SignIn();
                        if (userId == -1)
                        {
                            Console.WriteLine("Invalid username or password.");
                            return;
                        }
                        var user = Database.Instance.Get<User>(userId);
                        string prompt = $"{user?.FirstName} {user?.LastName} ({user?.UserName}) > ";
                        do
                        {
                            Console.Write(prompt);
                            input = Console.ReadLine();
                            HandleInput(input, user);
                        } while (true);
                        break;
                    case "createaccount":
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();
                        Console.Write("First Name: ");
                        string firstName = Console.ReadLine();
                        Console.Write("Last Name: ");
                        string lastName = Console.ReadLine();
                        Console.Write("National Code: ");
                        string nationalCode = Console.ReadLine();
                        Console.Write("Phone Number: ");
                        string phoneNumber = Console.ReadLine();
                        Console.Write("Initial Deposit Amount: ");
                        decimal initialDepositAmount;
                        while (!decimal.TryParse(Console.ReadLine(), out initialDepositAmount) || initialDepositAmount <= 0)
                        {
                            Console.Write("Please enter a valid initial deposit amount: ");
                        }
                        try
                        {
                            Services.AccountServices.CreateAccount(username, password, firstName, lastName, nationalCode, phoneNumber, initialDepositAmount);
                            Console.WriteLine("Account created successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating account: {ex.Message}");
                        }
                        break;
                }
            } while (true);
        }

        static void PrintAccounts()
        {
            foreach (var account in Database.Instance.FetchAll<Account>())
            {
                var user = Database.Instance.Get<User>(account.UserId);
                var card = Database.Instance.Get<DebitCard>(account.CardId);
                Console.WriteLine($"{user?.UserName} - {card?.CardNumber} - {account.AccountNumber}");
            }
        }
    }
}
