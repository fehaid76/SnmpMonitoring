using snmpMon.Engine.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snmpMon.Engine.Data
{
    public interface IDeviceRepository
    {
        Task AddDeviceAsync(DeviceData device);
        Task<List<DeviceData>> GetAllDevicesAsync();
    }
}