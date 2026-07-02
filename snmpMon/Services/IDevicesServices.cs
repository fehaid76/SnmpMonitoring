using snmpMon.Engine.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snmpMon.Services
{
    public interface IDevicesServices
    {
        
        Task<List<DeviceData>> GetAllDevicesAsync();
    }
}