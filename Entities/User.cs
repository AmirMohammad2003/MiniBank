using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Entities
{
    public class User: IDatabaseEntity
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
