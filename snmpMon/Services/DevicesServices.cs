using Microsoft.Extensions.DependencyInjection;
using snmpMon.Core.Models;
using snmpMon.Data.Model;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using snmpMon.Data;

namespace snmpMon.Services
{
    public class DevicesServices : IDevicesServices
    {
        public event Action<string> OnDeviceDeleted; 
        public event Action<Device> OnDeviceAdded;
        private readonly snmpMon.Data.Repositories.IDeviceRepository _deviceRepository;

        public DevicesServices(snmpMon.Data.Repositories.IDeviceRepository deviceRepository)
        {
            
            _deviceRepository = deviceRepository;


        }

        public async Task<bool> AddDeviceAsync(Device device)
        {
            var isAdded = await _deviceRepository.AddDeviceAsync(device);
            if (isAdded)
            {
                OnDeviceAdded?.Invoke(device);
            }

            return isAdded;
        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            return await _deviceRepository.GetAllDevicesAsync();

        }
        public async Task<bool> DeleteDeviceAsync(string ipAddress)
        {
            var isDeleted = await _deviceRepository.DeleteDeviceAsync(ipAddress);
            if (isDeleted)
            {
                OnDeviceDeleted?.Invoke(ipAddress);
            }

            return isDeleted;
        }

    }
}
