using MiniBank.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Validators
{
    public class UserValidator : IValidator<Entities.User>
    {
        public void Validate(User entity)
        {
            if (entity == null)
            {
                throw new ValidationError("User cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(entity.UserName))
            {
                throw new ValidationError("UserName cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(entity.Password))
            {
                throw new ValidationError("Password cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(entity.FirstName))
            {
                throw new ValidationError("FirstName cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(entity.LastName))
            {
                throw new ValidationError("LastName cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(entity.NationalCode))
            {
                throw new ValidationError("NationalCode cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(entity.PhoneNumber))
            {
                throw new ValidationError("PhoneNumber cannot be null or empty.");
            }
            if (entity.NationalCode.Length != 10)
            {
                throw new ValidationError("NationalCode must be exactly 10 characters long.");
            }
            if (entity.PhoneNumber.Length < 10 || entity.PhoneNumber.Length > 15)
            {
                throw new ValidationError("PhoneNumber must be between 10 and 15 characters long.");
            }
            if (entity.UserName.Length < 3 || entity.UserName.Length > 20)
            {
                throw new ValidationError("UserName must be between 3 and 20 characters long.");
            }
            if (entity.Password.Length < 6 || entity.Password.Length > 20)
            {
                throw new ValidationError("Password must be between 6 and 20 characters long.");
            }
            bool checkUniqueness = Database.Instance.Exists<User>(User =>
                User != null &&
                (User.UserName == entity.UserName || User.NationalCode == entity.NationalCode) &&
                User.Id != entity.Id);

            if (checkUniqueness) { 
                throw new ValidationError("UserName or NationalCode already exists.");
            }
        }
    }
}
