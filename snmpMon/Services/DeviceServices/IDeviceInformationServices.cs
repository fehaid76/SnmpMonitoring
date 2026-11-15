using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public interface IDeviceInformationServices
    {
        Task<(string sysName, string sysLocation, bool sysStatus)> GetSysDetails(string ipAddress, string communityString);
    }
}