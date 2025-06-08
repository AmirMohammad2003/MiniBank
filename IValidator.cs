using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank
{
    public interface IValidator<TEntity> where TEntity: IDatabaseEntity 
    {
        void Validate(TEntity entity);
    }
}
