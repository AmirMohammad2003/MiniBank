namespace MiniBank.Validators
{
    public class DebitCardValidator : IValidator<Entities.DebitCard>
    {
        public void Validate(Entities.DebitCard entity)
        {
            if (entity == null)
            {
                throw new ValidationError("Debit card cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(entity.CardNumber) || entity.CardNumber.Length != 16)
            {
                throw new ValidationError("Card number must be a 16-digit string.");
            }

            if (entity.ExpiryDate <= DateTime.UtcNow)
            {
                throw new ValidationError("Expiry date must be in the future.");
            }

            if (entity.Passcode < 1000 || entity.Passcode > 9999)
            {
                throw new ValidationError("Passcode must be a 4-digit number.");
            }

            if (entity.Passcode2 < 1000 || entity.Passcode2 > 9999)
            {
                throw new ValidationError("Passcode2 must be a 4-digit number.");
            }
        }
    }
}
