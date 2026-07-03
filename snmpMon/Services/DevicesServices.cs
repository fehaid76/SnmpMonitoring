using Microsoft.Extensions.DependencyInjection;
using snmpMon.Core.Models;
using snmpMon.Engine.Data;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Services
{
    public class DevicesServices : IDevicesServices
    {
        
        private readonly IDeviceRepository _deviceRepository;

        public DevicesServices(IDeviceRepository deviceRepository)
        {
            
            _deviceRepository = deviceRepository;


        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            return await _deviceRepository.GetAllDevicesAsync();

        }

    }
}
