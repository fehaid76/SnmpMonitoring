using Lextm.SharpSnmpLib;
using snmpMon.Core.Models;
using snmpMon.Data.Model;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace snmpMon.Core.Services
{
    public class DeviceInterfacesServices : IDeviceInterfacesServices
    {
        
        
        private DateTime _lastCheckTime = DateTime.MinValue;
        private long _lastDeviceBytes = 0;
        private double _lastSmoothedSpeed = 0;
        private List<DeviceActiveInterfaces> _deviceActiveInterfaces;
        private ISnmpService _snmpService;
        public DeviceInterfacesServices()
        {
            _snmpService = new SnmpService();
            _deviceActiveInterfaces = new List<DeviceActiveInterfaces>();
            
        }

        public double CalculatTotalDeviceTrafficSpeedKbps()
        {
            long currentTotalDevicesBytes = AllInterfacesBytes();
            DateTime now = DateTime.Now;

            if (_lastCheckTime == DateTime.MinValue || _lastDeviceBytes == 0)
            {
                _lastDeviceBytes = currentTotalDevicesBytes;
                _lastCheckTime = now;
                return 0;
            }

            double timeDifferent = (now - _lastCheckTime).TotalSeconds;
            if (timeDifferent <= 0)
                return 0;
            long bytesDelta = currentTotalDevicesBytes - _lastDeviceBytes;
            if (bytesDelta < 0)
                bytesDelta = 0;

            double speedKbps = (bytesDelta * 8.0) / (1024.0 * timeDifferent);

            _lastDeviceBytes = currentTotalDevicesBytes;
            _lastCheckTime = now;

            double alpha = 0.15; 
            if(_lastSmoothedSpeed == 0)
            {
                _lastSmoothedSpeed = speedKbps;
            }
            else
            {
                _lastSmoothedSpeed = (alpha * speedKbps) + ((1 - alpha) * _lastSmoothedSpeed);
            }

            return Math.Round(_lastSmoothedSpeed, 2);
        }

        public async Task DeviceTrafficSpeedKbps(List<Device> devices, string oid)
        {
            _deviceActiveInterfaces.Clear();
            foreach (Device device in devices)
            {
                try
                {
                    var _deviceAcInterfaces = new DeviceActiveInterfaces
                    {
                        DeviceName = device.Name,
                        IPAddress = device.IPAddress,
                        InOctets = await getInOctets(device.IPAddress, device.CommunityString, oid),
                    };
                    _deviceActiveInterfaces.Add(_deviceAcInterfaces);
                }
                catch (Exception ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }

            }
        }


        //public async Task DeviceTrafficSpeedKbps(List<Device> devices, string oid)
        //{
        //    _deviceActiveInterfaces.Clear();

        //    var tasks = devices.Select(async device =>
        //    {
        //        double octetsValue = 0;

        //        try
        //        {
        //            octetsValue = await getInOctets(device.IPAddress, device.CommunityString, oid);

        //        }
        //        catch (OperationCanceledException)
        //        {
        //            octetsValue = 0;
        //        }

        //        catch (Exception ex)
        //        {
        //            Logger.LogCriticalException(ex, ex.Message);

        //            octetsValue = 0;
        //        }

        //        return new DeviceActiveInterfaces
        //        {
        //            DeviceName = device.Name,
        //            IPAddress = device.IPAddress,
        //            InOctets = octetsValue
        //        };
        //    });

        //    var results = await Task.WhenAll(tasks);

        //    foreach (var result in results)
        //    {
        //        _deviceActiveInterfaces.Add(result);
        //    }
        //}


        private async Task<double> getInOctets(string ip, string communityString, string oid)
        {
            try
            {
                var IfInOctets = await _snmpService.GetWalkAsync(ip, communityString, oid);
                if (IfInOctets == null) return 0;
                double InOctetsValue = 0;

                foreach (var ifc in IfInOctets)
                {
                    if (ifc?.Data == null) continue;

                    string dataString = ifc.Data.ToString();
                    if (double.TryParse(dataString, out double currentValue))
                    {
                        if (currentValue > 0)
                            InOctetsValue += currentValue;
                    }

                }
                return InOctetsValue;
            }
            catch(Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
                return 0;
            }

            
        }

        private long AllInterfacesBytes()
        {
            long totalBytes = 0;
            foreach (var d in _deviceActiveInterfaces)
            {
                if(d.InOctets > 0)
                    totalBytes += (long)d.InOctets;
            }
            return totalBytes;
        }

        public string GetTopInboundDevice()
        {
            if (_deviceActiveInterfaces == null || !_deviceActiveInterfaces.Any())
                return "Unknown";
            var topDevice = _deviceActiveInterfaces
                .OrderByDescending(d => d.InOctets)
                .FirstOrDefault();

            return topDevice?.DeviceName ?? "Unknown";


        }




    }
}
