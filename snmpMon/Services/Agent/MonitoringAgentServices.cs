using snmpMon.Models;
using snmpMon.Services.AlertServices;
using snmpMon.Services.SensorServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace snmpMon.Services.Agent
{
    public class MonitoringAgentServices : IMonitoringAgentServices
    {
        private readonly IAlertNotify _alertNotify;
        private DeviceInfo _deviceInfo;
        private readonly IEnumerable<ISensor> _sensors; 
        public MonitoringAgentServices(IAlertNotify alertNotify, IEnumerable<ISensor> sensors)
        {
            _sensors = sensors;
            _alertNotify = alertNotify;

            EventSubsecribing();
        }

        public async Task StartMonitoring(string ipAddress, string communityString, DeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo;
            List<Task> tasks = new List<Task>();
            if (_deviceInfo.DeviceName == string.Empty)
                _deviceInfo.DeviceName = "Unknown device";
            foreach (var sensor in _sensors)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await sensor.PollAsync(ipAddress, communityString);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
                tasks.Add(task);


            }

            await Task.WhenAll(tasks);
            
        }

        private void EventSubsecribing()
        {
            var pingSensor = _sensors.OfType<ISensor<string, bool>>().FirstOrDefault();
            if (pingSensor != null)
            {
                pingSensor.OnSensorExecuted += (ip, isSucc) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        if (isSucc)
                        {
                            if (_deviceInfo.IsConnected == false)
                            {
                                _alertNotify.ShowSuccess($"{_deviceInfo.DeviceName} status is connected");
                                _deviceInfo.SuccessCount++;
                            }

                            _deviceInfo.IsConnected = true;
                        }
                        else
                        {
                            if(_deviceInfo.IsConnected == true)
                            {
                                _deviceInfo.ErrorsCount++;
                                _alertNotify.ShowErrors($"{_deviceInfo.DeviceName} status is disconnected");

                            }
                            _deviceInfo.IsConnected = false;


                        }
                    });
                };
            }

            var portsSensor = _sensors.OfType<ISensor<string, int>>().FirstOrDefault();
            if(portsSensor != null)
            {
                portsSensor.OnSensorExecuted += (ip, ports) =>
                {
                    Application.Current.Dispatcher.Invoke(() => _deviceInfo.PortsCount = ports);
                };
            };

            var deviceName = _sensors.OfType<ISensor<string, string>>().FirstOrDefault();
            if (deviceName != null)
            {
                deviceName.OnSensorExecuted += (ip, dvName) =>
                {
                    Application.Current.Dispatcher.Invoke(() => _deviceInfo.DeviceName = dvName);
                };
            };
        }


    }
}
