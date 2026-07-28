using Microsoft.Extensions.DependencyInjection;
using snmpMon.Core.Services;
using snmpMon.Data.Model;
using snmpMon.Engine.Helper;
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
using System.Windows.Threading;

namespace snmpMon.Services.Agent
{
    public class OverviewServices : IOverviewServices
    {
        public event Action<double> OnInboundKbps;
        public event Action<double> OnOutboundKbps;
        public event Action<string> OnTopInboundDevice;
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
        public async Task StartNetworkAverageLatencyCalculateAsync(List<Device> devicesIPs)
        {
            devices = devicesIPs;  
            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = new CancellationTokenSource();
            var token = _cancellation.Token;
            double ave = 0;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    ave = await pingToGetAverageLatencyAsync(devicesIPs);
                    OnAverageNetworkLatency?.Invoke(ave);
                    await Task.Delay(TimeSpan.FromMilliseconds(3000), token);
                }
            }
            catch (OperationCanceledException) { }

        }


        public async Task StartInboundTraffic()
        {
            var interfacesServices = App.serviceProvider.GetRequiredService<IDeviceInterfacesServices>();
            //await Task.Delay(3000);
            var Innode = await _mibParser.GetNode("ifInOctets");
            

            double inTraaffic = 0;
            _inboundTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(4)
            };

            _inboundTimer.Tick += async (s, e) =>
            {
                await interfacesServices.DeviceInboundSpeedKbps(devices, Innode.Oid);
                inTraaffic = interfacesServices.CalculatTotalDeviceInboundSpeedKbps();

                OnInboundKbps?.Invoke(inTraaffic);
                getTopTrafficConsumerDevice(interfacesServices);


            };

            _inboundTimer.Start();
        }
        public async Task StartOutboundTraffic()
        {
            var interfacesServices = App.serviceProvider.GetRequiredService<IDeviceInterfacesServices>();

            await Task.Delay(3000);
            var outboundNode = await _mibParser.GetNode("ifOutOctets");

            double outboundTraffic = 0;

            _outboundTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            _outboundTimer.Tick += async (s, e) =>
            {

                await interfacesServices.DeviceInboundSpeedKbps(devices, outboundNode.Oid);
                double outboundTraffic = interfacesServices.CalculatTotalDeviceInboundSpeedKbps();
                OnOutboundKbps?.Invoke(outboundTraffic);

            };

            _outboundTimer.Start();
        }

        private void getTopTrafficConsumerDevice(IDeviceInterfacesServices interfacesServices)
        {
            var consumerDevice = interfacesServices.GetTopInboundDevice();
            if (!string.IsNullOrEmpty(consumerDevice))
            {
                OnTopInboundDevice?.Invoke(consumerDevice);
            }
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
