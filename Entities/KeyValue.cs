namespace MiniBank.Entities
{
    public class KeyValue(string key, string value) : IDatabaseEntity
    {
        public long Id { get; set; }
        public string? Key { get; init; } = key;
        public string? Value { get; set; } = value;
    }
}
