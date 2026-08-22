using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Extensions;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.SensorServices
{
    public class InterfacesSensors : IInterfacesSensors
    {
        public event EventHandler<TrafficEventArgs> TrafficDataReady;
        private string _ipAddress { get; set; }
        private string _communityString { get; set; }
        private string _ifIndex { get; set; }
        private DateTime _lastTrafficOutVolumeTime { get; set; }

        private readonly IMibParser _mibParser;
        private readonly ISnmpService _snmpService;
        public InterfacesSensors(IMibParser mibParser, ISnmpService snmpService)
        {
            _mibParser = mibParser;
            _snmpService = snmpService;
        }

        public async Task StartFetchInterfaceDetails(string ipAddress, string communityString, string ifIndex)
        {
            _ipAddress = ipAddress;
            _communityString = communityString;
            _ifIndex = ifIndex;

            await startFetchDataProccessAsync();
        }

        private async Task<(string trafficInVolume, string trafficOutVolume)> getTrafficNodes()
        {
            try
            {
                var fetchTrafficInTask = _mibParser.GetNode("ifInOctets");
                var fetchTrafficOutTask = _mibParser.GetNode("ifOutOctets");
                await Task.WhenAll(fetchTrafficInTask, fetchTrafficOutTask);

                MibNode trafficInNode = await fetchTrafficInTask;
                MibNode trafficOutNode = await fetchTrafficOutTask;
                return (trafficInNode.Oid, trafficOutNode.Oid);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return (string.Empty, string.Empty);
        }
        private async Task startFetchDataProccessAsync()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
            var trafficVolumeNodes = await getTrafficNodes();
            string trafficInOid = trafficVolumeNodes.trafficInVolume;
            string trafficOutOid = trafficVolumeNodes.trafficOutVolume;
            if(string.IsNullOrEmpty(trafficInOid) || string.IsNullOrEmpty(trafficOutOid))
            {
                Logger.LogCriticalException(new Exception("OIDs could not be resolved"), "Stopping sensor process because MIB nodes are missing.");
                return;
            }

            do
            {
                try
                {
                    await fetchAndRaiseTrafficDataAsync(trafficInOid, trafficOutOid);

                }
                catch (Exception ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }

            } while (await timer.WaitForNextTickAsync());

        }

        private async Task fetchAndRaiseTrafficDataAsync(string trafficInOid, string trafficOutOid)
        {
            var trafficInTask = GetTrafficVolumeAsync(trafficInOid);
            var trafficOutTask = GetTrafficVolumeAsync(trafficOutOid);

            await Task.WhenAll(trafficInTask, trafficOutTask);

            long trafficIn = await trafficInTask;
            long trafficOut = await trafficOutTask;

            TrafficDataReady?.Invoke(this, new TrafficEventArgs
            {
                TrafficIn = trafficIn,
                TrafficOut = trafficOut,
            });
        }

        private async Task<long> GetTrafficVolumeAsync(string oid)
        {
            try
            {
                var trafficOut = await _snmpService.GetLongAsync(_ipAddress, _communityString, oid.AddDigit(_ifIndex));
                return trafficOut;
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return 0;


        }




    }
}
