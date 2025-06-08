using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Entities
{
    public class KeyValue(string key, string value) : IDatabaseEntity
    {
        public long Id { get; set; }
        public string? Key { get; set; } = key;
        public string? Value { get; set; } = value;
    }
}
