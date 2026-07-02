using snmpMon.Models;
using System.Threading.Tasks;

namespace snmpMon.Services.Agent
{
    public interface IMonitoringAgentServices
    {
        Task StartMonitoring(string ipAddress, string communityString, DeviceInfo deviceInfo);
    }
}