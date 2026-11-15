
using snmpMon.MibParse.Models;

namespace snmpMon.Core.Services
{
    public interface IDeviceAvailabilityChecker
    {
        event Action<string, string> DevicePowerRestored;

        Task StartDeviceChecker(string ipAddress, string communityString);
    }
}