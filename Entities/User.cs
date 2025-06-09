namespace MiniBank.Entities
{
    [ValidatorAttribute(typeof(Validators.UserValidator))]
    public class User : IDatabaseEntity
    {
        public long Id { get; set; }
        public string UserName { get; init; }
        public string Password { get; set; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string NationalCode { get; init; }
        public string PhoneNumber { get; set; }
    }
}
