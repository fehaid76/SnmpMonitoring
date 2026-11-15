using Microsoft.Extensions.DependencyInjection;
using snmpMon.Core.Services;
using snmpMon.Data.Model;
using snmpMon.Engine.Helper;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace snmpMon.Services.Agent
{
    public class OverviewServices : IOverviewServices
    {
        public event EventHandler<DeviceTrafficEventArgs> DeviceTrafficReceived;
        public event Action<double> OnAverageNetworkLatency;
        public event Action<int> OnUpDevices;
        private List<Device> devices = new List<Device>();
        private DispatcherTimer _inboundTimer;
        private DispatcherTimer _outboundTimer;
        private Task<long?>[] _reusablePingTasks = Array.Empty<Task<long?>>();


        CancellationTokenSource _cancellation;
        IDeviceInterfacesServices _deviceInterfacesServices;
        IMibParser _mibParser;
    
        public OverviewServices(IDeviceInterfacesServices deviceInterfacesServices, IMibParser mibParser)
        {
            _deviceInterfacesServices = deviceInterfacesServices;
            _mibParser = mibParser;
            
            
            
        }
        public async Task StartNetworkAverageLatencyCalculateAsync(List<Device> devicesIPs, CancellationTokenSource cancellationToken = null)
        {
            devices = devicesIPs;

            _cancellation = cancellationToken != null ? cancellationToken : new CancellationTokenSource();
            var token = _cancellation.Token;
            double ave = 0;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    ave = await pingToGetAverageLatencyAsync(devicesIPs);
                    OnAverageNetworkLatency?.Invoke(ave);
                    await Task.Delay(TimeSpan.FromMilliseconds(3000), token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch (OperationCanceledException ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }

            }


        }

        public void StartTrafficMonitoring(CancellationTokenSource cancellationToken = null)
        {
            var _cancellation = cancellationToken != null ? cancellationToken: new CancellationTokenSource();
            _ = StartInboundTraffic(_cancellation);

        }
        private async Task StartInboundTraffic(CancellationTokenSource cancellationToken)
        {

            _cancellation = cancellationToken;
            var token = _cancellation.Token;

            var interfacesServices = App.serviceProvider.GetRequiredService<IDeviceInterfacesServices>();
            var InNode = await _mibParser.GetNode("ifInOctets");
            var outboundNode = await _mibParser.GetNode("ifOutOctets");


            using var periodicTime = new PeriodicTimer(TimeSpan.FromSeconds(5));

            while (await periodicTime.WaitForNextTickAsync(token))
            {
                try
                {
                    await interfacesServices.DeviceTrafficSpeedKbps(devices, InNode.Oid);
                    double inTraaffic = interfacesServices.CalculatTotalDeviceTrafficSpeedKbps();
                    await interfacesServices.DeviceTrafficSpeedKbps(devices, outboundNode.Oid);
                    double outboundTraffic = interfacesServices.CalculatTotalDeviceTrafficSpeedKbps();

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var topConsumerDevice = getTopTrafficConsumerDevice(interfacesServices);
                        DeviceTrafficReceived?.Invoke(this, new DeviceTrafficEventArgs(inTraaffic, outboundTraffic, topConsumerDevice));
                    });
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch (OperationCanceledException ex)
                {

                    Logger.LogCriticalException(ex, ex.Message);
                }

                catch (Exception ex) when (ex is not OperationCanceledException)
                {

                    Logger.LogCriticalException(ex, ex.Message);

                }
            }




        }

        private string  getTopTrafficConsumerDevice(IDeviceInterfacesServices interfacesServices)
        {
            var consumerDevice = interfacesServices.GetTopInboundDevice();
            if (!string.IsNullOrEmpty(consumerDevice))
            {
                return consumerDevice;
            }

            return "N/A";
        }

        public void MemoryCleaner()
        {
            _inboundTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(3)
            };

            _inboundTimer.Tick += (s, e) =>
            {
                GC.Collect(2, GCCollectionMode.Optimized, false);
                
            };

            _inboundTimer.Start();
        }

        private async Task<double> pingToGetAverageLatencyAsync(List<Device> devicesIPs)
        {
            if (devicesIPs == null || !devicesIPs.Any())
                return 0;
            int totalDevices = devicesIPs.Count;
            if(_reusablePingTasks == null || _reusablePingTasks.Length != totalDevices)
            {
                _reusablePingTasks = new Task<long?>[totalDevices];

            }

            for (int i = 0; i < totalDevices; i++)
            {
                _reusablePingTasks[i] = SendSinglePingAsync(devicesIPs[i].IPAddress);
            }

            var result = await Task.WhenAll(_reusablePingTasks);

            long totalLatency = 0;
            int activeCount = 0;

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].HasValue)
                {
                    totalLatency += result[i].Value;
                    activeCount++;
                }
            }

            OnUpDevices?.Invoke(activeCount);

            return activeCount > 0 ? Math.Round((double)totalLatency / activeCount, 2) : 0;

        }

        private async Task<long?> SendSinglePingAsync(string ip)
        {
            using (var ping = new Ping())
            {
                try
                {
                    var replay = await ping.SendPingAsync(ip, 2000);
                    if (replay.Status == IPStatus.Success)
                        return (long?)replay.RoundtripTime;
                }
                catch (Exception ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }
            }

            return null;
        }

    }
}
