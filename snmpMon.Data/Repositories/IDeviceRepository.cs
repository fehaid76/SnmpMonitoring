using snmpMon.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Data.Repositories
{
    public interface IDeviceRepository
    {
        Task<bool> AddDeviceAsync(Device device);
        Task<List<Device>> GetAllDevicesAsync();
    }
}
