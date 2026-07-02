using System.Threading.Tasks;

namespace snmpMon.Sensors.SensorsServices
{
    public interface IDefaultSensors
    {
        Task<string> GetDeviceName(string ipAddress, string communityString);
        Task<int> GetPortsCountstring(string iPAddress, string communityString);
        Task<bool> PingSensorAsync(string iPAddress);
    }
}