using System;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public interface IDeviceUptimeService
    {
        event Action<TimeSpan> OnUpTimeChanged;
        Task StartTrackingAsync(string ipAddress, string communityString, CancellationToken cancellationToken = default);
    }
}