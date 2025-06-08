using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniBank.Entities;

namespace MiniBank.Services
{
    public static class AccountServices
    {
        public static void CreateAccount(string UserName, string Password, string FirstName, string LastName, string NationalCode, string PhoneNumber, decimal initialDepositAmount)
        {
            if (initialDepositAmount <= 0)
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

            DebitCard card = new();
            Account account = new(user, card.Id); 
            Deposit initialDeposit = new(account, initialDepositAmount);
            account.Balance += initialDeposit.Amount;

            Database.Instance.Save(user);
            Database.Instance.Save(card);
            Database.Instance.Save(account);
            Database.Instance.Save(initialDeposit);
            
        }
    }
}
