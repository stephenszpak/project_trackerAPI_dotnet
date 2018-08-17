using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.DAL.Async
{
    internal class IteratorSimpleAsync<TDelegate> : IIteratorAsync<TDelegate>
    {
        private YieldCallbackAsync<TDelegate> yieldAsync;

        internal IteratorSimpleAsync(YieldCallbackAsync<TDelegate> yieldAsync)
        {
            this.yieldAsync = yieldAsync;
        }

        #region IIteratorAsync

        public async Task IterateAsync(TDelegate callback)
        {
            await yieldAsync(callback);
        }

        #endregion
    }
}
