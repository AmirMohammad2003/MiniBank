namespace MiniBank
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ValidatorAttribute(Type validatorType) : Attribute
    {
        private readonly Type _validatorType = validatorType;

        public Type ValidatorType
        {
            get { return _validatorType; }
        }
    }
}
