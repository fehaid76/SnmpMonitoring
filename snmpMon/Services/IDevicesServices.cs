using snmpMon.Data.Model;
using snmpMon.Engine;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snmpMon.Services
{
    public interface IDevicesServices
    {
        event Action<string> OnDeviceDeleted;
        event Action<Device> OnDeviceAdded;
        Task<bool> AddDeviceAsync(Device device);
        Task<List<Device>> GetAllDevicesAsync();
        Task<List<DeviceEndpoint>> GetAllEndPointsDetails();
        Task<bool> DeleteDeviceAsync(string ipAddress);
        Task<List<DeviceAlert>> GetAllMessagesAsync();

    }
}