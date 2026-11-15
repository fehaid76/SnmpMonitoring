using snmpMon.Engine.Services.SnmpServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public class DeviceInformationServices : IDeviceInformationServices
    {
        private string _ipAddress;
        private string _communityString;
        private readonly ISnmpService _snmpService;
        public DeviceInformationServices(ISnmpService snmpService)
        {
            _snmpService = snmpService;
        }

        private async Task<string> GetSysDetails(string oid)
        {
            var sysDesc = await _snmpService.GetStringAsync(_ipAddress, _communityString, oid);
            if (!string.IsNullOrEmpty(sysDesc))
                return sysDesc;
            return "Undefind";
        }

        public async Task<(string sysName, string sysLocation, bool sysStatus)> GetSysDetails(string ipAddress, string communityString)
        {
            PrepareSNMPDetails(ipAddress, communityString);
           //var processDuration = TimeSpan.FromSeconds(5);
           var systemName = await GetSysDetails("1.3.6.1.2.1.1.5.0");
            var systemLocation = await GetSysDetails("1.3.6.1.2.1.1.6.0");
            var systemStatus = !string.IsNullOrEmpty(systemName) || !string.IsNullOrEmpty(systemLocation);
            return (systemName, systemLocation, systemStatus);
        }

        private void PrepareSNMPDetails(string ipAddress, string communityString)
        {
            _ipAddress = ipAddress;
            _communityString = communityString;
        }
    }
}
