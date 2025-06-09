using MiniBank.Entities;

namespace MiniBank.Services
{
    public static class AccountServices
    {
        public static event Action<User>? OnSignIn;

        public static long SignIn(string UserName, string Password)
        {
            var user = Database.Instance.Filter<User>(u => u.UserName == UserName.Trim() && u.Password == Password.Trim()).FirstOrDefault();
            if (user != null)
            {
                OnSignIn?.Invoke(user);
                return user.Id; // Wow WoW woW.
            }
            else
            {
                throw new ArgumentException("Invalid username or password.");
            }
        }


        public static void CreateAccount(string UserName, string Password, string FirstName, string LastName, string NationalCode, string PhoneNumber, decimal initialDepositAmount)
        {
            if (initialDepositAmount <= 100)
            {
                throw new ArgumentException("Initial deposit amount must be greater than zero.", nameof(initialDepositAmount));
            }

            User user = new()
            {
                UserName = UserName.Trim(),
                Password = Password.Trim(),
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                NationalCode = NationalCode.Trim(),
                PhoneNumber = PhoneNumber.Trim()
            };
            Database.Instance.Save(user);

            DebitCard card = new();
            Database.Instance.Save(card);

            Account account = new(user, card.Id);
            Database.Instance.Save(account);

            Deposit initialDeposit = new(account, initialDepositAmount);
            Database.Instance.Save(initialDeposit);

            account.Deposit(initialDeposit.Amount);
            Database.Instance.Update(account);

        }

        public static decimal AccountBalance(long accountId)
        {
            var account = Database.Instance.Get<Account>(accountId) ?? throw new ArgumentException("Account not found.", nameof(accountId));
            return account.Balance;
        }

        public static void Deposit(long accountId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
            }

            var account = Database.Instance.Get<Account>(accountId) ?? throw new ArgumentException("Account not found.", nameof(accountId));
            var deposit = new Deposit(account, amount);
            account.Deposit(amount);

            Database.Instance.Save(deposit);
            Database.Instance.Update(account);
            string logMessage = $"Deposit of {amount} from account {accountId} successful. New balance: {account.Balance}";
            File.AppendAllText("log.txt", $"{DateTime.Now}: {logMessage}");
        }

        public static void Withdraw(long accountId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));
            }

            var account = Database.Instance.Get<Account>(accountId) ?? throw new ArgumentException("Account not found.", nameof(accountId));
            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds for withdrawal.");
            }

            account.Withdraw(amount);
            Database.Instance.Update(account);
            string logMessage = $"Withdrawal of {amount} from account {accountId} successful. New balance: {account.Balance}";
            File.AppendAllText("log.txt", $"{DateTime.Now}: {logMessage}");
        }

        public static List<Transaction> GetTransactions(long accountId)
        {
            var account = Database.Instance.Get<Account>(accountId) ?? throw new ArgumentException("Account not found.", nameof(accountId));
            return Database.Instance.Filter<Transaction>(t => t.FromAccountId == account.Id || t.ToAccountId == account.Id).ToList();
        }

        private static void SendPasscode(long accountId, long recivingAccountId, decimal amount)
        {

            int passcode = new Random().Next(100000, 999999);
            PassCode code = new(passcode.ToString(), accountId, recivingAccountId, amount);
            Database.Instance.Save(code);
            string logMessage = $"Randomly generated passcode: {passcode}";
            File.AppendAllText("log.txt", $"{DateTime.Now}: {logMessage}");

        }

        public static bool CheckPayment(long accountId, long recivingAccountId, decimal amount)
        {
            var account = Database.Instance.Get<Account>(accountId);
            var recivingAccount = Database.Instance.Get<Account>(recivingAccountId);
            if (account == null || recivingAccount == null || account.Balance < amount)
            {
                return false;
            }

            if (amount > 100)
            {
                SendPasscode(accountId, recivingAccountId, amount);
            }

            return true;
        }

        public static bool ContinuePayment(long accountId, long recivingAccountId, decimal amount, int userEnteredPasscode)
        {
            var account = Database.Instance.Get<Account>(accountId);
            var recivingAccount = Database.Instance.Get<Account>(recivingAccountId);
            if (account!.Balance < amount)
            {
                return false;
            }
            var card = Database.Instance.Get<DebitCard>(account.CardId);
            if (card == null)
            { // this can't be
                return false;
            }

            int passcode = card.Passcode2;

            PassCode? code = null;
            if (amount > 100)
            {
                var codes = Database.Instance.Filter<PassCode>(c => c.AccountId == accountId && c.RecivingAccountId == recivingAccountId && c.Amount == amount);
                List<PassCode> toDelete = new();
                foreach (var codeInCheck in codes)
                {
                    if (codeInCheck.IsValid())
                    {
                        code = codeInCheck;
                        // break;
                    }
                    else
                    {
                        toDelete.Add(codeInCheck);
                    }
                }
                toDelete.ForEach(c => Database.Instance.Delete(c));

                if (code == null)
                {
                    return false; // No valid passcode found
                }
                passcode = Convert.ToInt32(code.Code);
            }

            if (passcode != userEnteredPasscode)
            {
                return false;
            }
            Database.Instance.Delete(code!);

            account.Withdraw(amount);
            recivingAccount!.Deposit(amount);

            Transaction transaction = new()
            {
                FromAccountId = account.Id,
                ToAccountId = recivingAccount.Id,
                Amount = amount,
                Description = $"Payment from {accountId} to {recivingAccountId}"
            };

            Database.Instance.Save(transaction);
            Database.Instance.Update(account);
            Database.Instance.Update(recivingAccount);
            string logMessage = $"Payment of {amount} from account {accountId} to {recivingAccountId} successful. New balance: {account.Balance}";
            File.AppendAllText("log.txt", $"{DateTime.Now}: {logMessage}");
            return true;
        }

    }
}
