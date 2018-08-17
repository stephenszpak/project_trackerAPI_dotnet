using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL
{
    public interface IDataContext
    {
        ITasks Tasks { get; }
    }
}
