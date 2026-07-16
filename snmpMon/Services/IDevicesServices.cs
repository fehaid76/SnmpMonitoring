using snmpMon.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;
using snmpMon.Models;
using snmpMon.Data.Model;

namespace snmpMon.Services
{
    public interface IDevicesServices
    {
        
        Task<List<Device>> GetAllDevicesAsync();
        Task<bool> DeleteDeviceAsync(string ipAddress);
    }
}