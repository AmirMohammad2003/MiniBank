using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank
{
    public interface IDatabaseEntity
    {
        long Id { get; set; }
    }
}
