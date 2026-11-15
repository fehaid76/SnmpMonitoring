using ScottPlot.Colormaps;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Extensions;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool _isPortSpeedRecieved {  get; set; }
        #region PreviousValues
        private long _previousTrafficIn;
        private long _previousTrafficOut;
        private DateTime _lastFetchTime = DateTime.MinValue;
        #endregion

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

        private double HandleTimePoll(DateTime currentTime)
        {
            double elapsedTime = 30.0;

            if (_lastFetchTime != DateTime.MinValue)
            {
                elapsedTime = (currentTime - _lastFetchTime).TotalSeconds;
                if (elapsedTime <= 0) elapsedTime = 1.0;
            }
            _lastFetchTime = currentTime;
            return elapsedTime;
        }

        private async Task<int> GetPortSpeed()
        {
            
            var mibNode = await _mibParser.GetNode("ifSpeed");
            if (mibNode != null && !_isPortSpeedRecieved)
            {
                _isPortSpeedRecieved = true;
                return await _snmpService.GetIntAsync(_ipAddress, _communityString, mibNode.Oid.ToString().AddDigit(_ifIndex));

            }
            return 1000000;
        }
        private async Task fetchAndRaiseTrafficDataAsync(string trafficInOid, string trafficOutOid)
        {

            try
            {

                var currentTime = DateTime.Now;
                var elapsedTime = HandleTimePoll(currentTime);

                var trafficValues = await GetTrafficValuesAsync(trafficInOid, trafficOutOid);

                if (trafficValues.TrafficInValue < 0 || trafficValues.TrafficOutValue < 0)
                {

                    return;
                }
                var differenceValues = GetTrafficValuesDifferenceInBytes(trafficValues.TrafficInValue, trafficValues.TrafficOutValue);
                var speedsValues = GetSpeedsInKilobitsPerSecond(differenceValues.TrafficIn, differenceValues.TrafficOut, elapsedTime);
                var trafficVolumes = GetTrafficVolumesInKilobytes(differenceValues.TrafficIn, differenceValues.TrafficOut);
                if (CheckForCachePreviousTrafficValues())
                {
                    TrafficDataReady?.Invoke(this, new TrafficEventArgs
                    {
                        TrafficInVolume = trafficVolumes.VolumeIn,
                        TrafficOutVolume = trafficVolumes.VolumeOut,
                        TrafficInSpeed = speedsValues.TrafficInSpeed,
                        TrafficOutSpeed = speedsValues.TrafficOutSpeed,
                        InterfaceSpeed = await GetPortSpeed(),
                        Timestamp = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss"),
                    });
                }

                CachePreviousTrafficValues(trafficValues.TrafficInValue, trafficValues.TrafficOutValue);

            }
            catch (Exception ex)
            {

                Logger.LogCriticalException(ex, ex.Message);
            }
        }

        private (double VolumeIn, double VolumeOut) GetTrafficVolumesInKilobytes(long volumeIn, long volumeOut)
        {
            double trafficVolIn = (double)ConvertBytesToKilobytes(volumeIn);
            double trafficVolOut = (double)ConvertBytesToKilobytes(volumeOut);
            return (trafficVolIn, trafficVolOut);

        }

        private bool CheckForCachePreviousTrafficValues()
        {
            return (_previousTrafficIn != 0 && _previousTrafficOut != 0);
        }

        private void CachePreviousTrafficValues(long trafficIn, long trafficOut)
        {
            _previousTrafficIn = trafficIn;
            _previousTrafficOut = trafficOut;
        }

        private async Task<(long TrafficInValue, long TrafficOutValue)> GetTrafficValuesAsync(string trafficInOid, string trafficOutOid)
        {
            try
            {
                var timeoutDuration = TimeSpan.FromSeconds(5);
                var trafficInTask = GetTrafficValueAsync(trafficInOid).WaitAsync(timeoutDuration);
                var trafficOutTask = GetTrafficValueAsync(trafficOutOid).WaitAsync(timeoutDuration);

                await Task.WhenAll(trafficInTask, trafficOutTask);

                long trafficIn = await trafficInTask;
                long trafficOut = await trafficOutTask;

                return (trafficIn, trafficOut);
            }
            catch(Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
                return (0, 0);
            }

        }

        private (long TrafficIn, long TrafficOut) GetTrafficValuesDifferenceInBytes(long trafficIn, long trafficOut)
        {
            trafficIn = CalculateByteDifference(trafficIn, _previousTrafficIn);
            trafficOut = CalculateByteDifference(trafficOut, _previousTrafficOut);
            return (trafficIn, trafficOut);
        }

        private (double TrafficInSpeed, double TrafficOutSpeed) GetSpeedsInKilobitsPerSecond(long byteDifferenceIn , long byteDifferenceOut, double elapsedTime)
        {
            var trafficIn = CalculateSpeedInKilobitsPerSecond(byteDifferenceIn, elapsedTime);
            var trafficOut = CalculateSpeedInKilobitsPerSecond(byteDifferenceOut, elapsedTime);
            return (trafficIn, trafficOut);

        }

        private double CalculateSpeedInKilobitsPerSecond(long byteDifference, double elapsedTime)
        {
            if (byteDifference <= 0)
                return 0;

            return Math.Round((byteDifference / elapsedTime * 8) / 1024, 3);
        }

        private long CalculateByteDifference(long currentValue, long previousValue)
        {
            if (currentValue >= previousValue)
            {
                return (currentValue - previousValue);

            }
            else
            {
                return (4294967296L - previousValue) + currentValue;
            }

        }

        private double ConvertBytesToKilobytes(long bytes)
        {

            var val = (double)bytes / 1024;
            return Math.Round(val, 3);
        }

        private async Task<long> GetTrafficValueAsync(string oid)
        {
            try
            {
                var snmpValue = await _snmpService.GetLongAsync(_ipAddress, _communityString, oid.AddDigit(_ifIndex));
                return snmpValue;
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
                return -1;
            }
        }




    }
}
