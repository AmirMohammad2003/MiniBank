using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Validators
{
    public class DepositValidator :  IValidator<Entities.Deposit>
    {
        public void Validate(Entities.Deposit entity)
        {
            if (entity == null)
            {
                throw new ValidationError("Deposit cannot be null.");
            }

            if (entity.Amount <= 0)
            {
                throw new ValidationError("Account ID must be greater than zero.");
            }
        }
    }
}
