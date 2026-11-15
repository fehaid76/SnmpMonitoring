using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.MibJson.Extenstions;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public class DeviceUptimeService : IDeviceUptimeService
    {
        public event Action<TimeSpan> OnUpTimeChanged;
        private readonly ISnmpService _snmpService;
        private readonly IMibParser _mibParser;
        public DeviceUptimeService(ISnmpService snmpService, IMibParser mibParser)
        {
            _snmpService = snmpService;
            _mibParser = mibParser;
        }

        private async Task<MibNode> GetUpTimeNode()
        {
            var upTimeNode = await _mibParser.GetNode("sysUpTime");
            return upTimeNode;
        }

        private async Task<TimeSpan> GetUpTime(string ipAddress, string communityString)
        {
            var upTimeOid = await GetUpTimeNode();
            var deviceUpTime = await _snmpService.GetUpTime(ipAddress, communityString, upTimeOid.Oid.ToScalarOid());
            OnUpTimeChanged?.Invoke(deviceUpTime);
            return deviceUpTime;
        }

        public async Task StartTrackingAsync(string ipAddress, string communityString, CancellationToken cancellationToken = default)
        {
            var upTime = await GetUpTime(ipAddress, communityString);

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            try
            {
                while (await timer.WaitForNextTickAsync())
                {
                    upTime += TimeSpan.FromSeconds(1);
                    OnUpTimeChanged?.Invoke(upTime);
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }
        }
    }
}
