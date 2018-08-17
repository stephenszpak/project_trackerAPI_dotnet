using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL.Async
{
    public interface IEnumeratorAsync<TDelegate> : IDisposable
    {
        Task<bool> MoveNextAsync(TDelegate callback);
        Task ResetAsync();
    }
}
