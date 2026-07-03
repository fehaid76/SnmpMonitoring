using snmpMon.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;
using snmpMon.Models;

namespace snmpMon.Services
{
    public interface IDevicesServices
    {
        
        Task<List<Device>> GetAllDevicesAsync();
    }
}