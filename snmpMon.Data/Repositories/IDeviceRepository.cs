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
        Task<bool> AddAlertMessageAsync(string AlertMessage, int AlertLevel, string AlertTime, string IPAddress);
        Task<List<Device>> GetAllDevicesAsync();
        Task<List<DeviceEndpoint>> GetDeviceEndpointsAsync();
        Task<List<DeviceAlert>> GetAllMessagesAsync(string IPAddress);
        Task<bool> DeleteDeviceAsync(string ipAddress);
        Task<long> AddTemplatePathAsync(string Path);
        Task<bool> AddCategoryAsync(string Category, long ReleatedID);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<string>> GetTemplatePathsAsync();
    }
}
