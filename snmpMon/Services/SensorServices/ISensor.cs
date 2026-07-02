using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.SensorServices
{
    public interface ISensor : IDisposable
    {
        bool isRunning { get; set; }
        CancellationTokenSource _cts { get; set; }


        Task PollAsync(string ipAddress, string communityString);

    }

    public interface ISensor<T, D> : ISensor
    {
        event Action<T, D> OnSensorExecuted;

    }
}
