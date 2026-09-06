using snmpMon.Engine.Helper;
using snmpMon.Helper;
using snmpMon.Helper.Model;
using snmpMon.Models;
using snmpMon.Services.AlertServices;
using snmpMon.Services.SensorServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Logger = snmpMon.Helper.Logger;

namespace snmpMon.Services.Agent
{
    public class MonitoringAgentServices : IMonitoringAgentServices
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private ActionDisposable ActionDisposable = null;
        private ActionDisposableAggregation disposableAggregation = new ActionDisposableAggregation();

        private DeviceInfo _deviceInfo;
        private readonly IEnumerable<ISensor> _sensors; 
        public MonitoringAgentServices( IEnumerable<ISensor> sensors)
        {
            _sensors = sensors;

            EventSubsecribing();
        }

        public async Task<ActionDisposableAggregation> StartMonitoring(string ipAddress, string communityString, DeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo ?? new DeviceInfo();
            if (string.IsNullOrWhiteSpace(_deviceInfo.DeviceName))
                _deviceInfo.DeviceName = "Unknown device";

            var pollTask = _sensors.Select(async sensor =>
            {
                try
                {
                    await sensor.PollAsync(ipAddress, communityString, _cts.Token);
                }
                catch (OperationCanceledException) { }
                catch(Exception ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }
            });

            await Task.WhenAll(pollTask);
            return preparedDisposableAggregation(ipAddress, ActionDisposable);


        }

        private ActionDisposableAggregation preparedDisposableAggregation(string ipAddress, ActionDisposable actionDisposable)
        {
            disposableAggregation.IPAddress = ipAddress;
            disposableAggregation.actionDisposable = actionDisposable;
            return disposableAggregation;
        }

        private void EventSubsecribing()
        {
            Action cleanupAction = null;  
            var pingSensor = _sensors.OfType<ISensor<string, bool>>().FirstOrDefault();
            if (pingSensor != null)
            {

                Action<string, bool> pingHandler = (ip, isSucc) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        if (isSucc)
                        {


                            _deviceInfo.IsConnected = true;
                        }
                        else
                        {

                            _deviceInfo.IsConnected = false;


                        }
                    });
                };

                if (pingSensor != null)
                {
                    pingSensor.OnSensorExecuted += pingHandler;
                    cleanupAction += () =>
                    {
                        pingSensor.OnSensorExecuted -= pingHandler;

                        ProcessCancel();

                    };
                }


            }



            var portsSensor = _sensors.OfType<ISensor<string, int>>().FirstOrDefault();
            if(portsSensor != null)
            {

                Action<string, int> portsHandler = (ip, ports) =>
                {
                    Application.Current.Dispatcher.Invoke(() => _deviceInfo.PortsCount = ports);
                };

                if(portsSensor != null)
                {
                    portsSensor.OnSensorExecuted += portsHandler;
                    cleanupAction += () =>
                    {
                        portsSensor.OnSensorExecuted -= portsHandler;
                        ProcessCancel();
                    };
                    
                }
            };


            ActionDisposable = new ActionDisposable(cleanupAction);
            
            

            //var deviceName = _sensors.OfType<ISensor<string, string>>().FirstOrDefault();
            //if (deviceName != null)
            //{
            //    deviceName.OnSensorExecuted += (ip, dvName) =>
            //    {
            //        Application.Current.Dispatcher.Invoke(() => _deviceInfo.DeviceName = dvName);
            //    };
            //};
        }

        private void ProcessCancel()
        {
            if(_cts != null)
            {
                var cts = _cts;
                _cts = null;
                try
                {
                    if (!cts.IsCancellationRequested)
                    {
                        cts.Cancel();
                    }
                }
                catch (ObjectDisposedException) { }
                finally
                {
                    cts.Dispose();
                }
            }
        }


    }
}
