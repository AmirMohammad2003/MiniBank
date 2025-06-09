namespace MiniBank
{
    public class ValidationError : Exception
    {
        public ValidationError() { }
        public ValidationError(string message) : base(message) { }
        public ValidationError(string message, Exception innerException) : base(message, innerException) { }
    }
}
