using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
