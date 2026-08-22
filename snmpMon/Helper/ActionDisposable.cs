using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Helper
{
    public class ActionDisposable : IDisposable
    {
        private Action _disposeActione;

        public ActionDisposable(Action disposeAcction)
        {
            _disposeActione = disposeAcction ?? throw new ArgumentNullException(nameof(disposeAcction));
            
        }

        public void Dispose()
        {
            var action = Interlocked.Exchange(ref _disposeActione, null);
            action?.Invoke();
        }
    }
}
