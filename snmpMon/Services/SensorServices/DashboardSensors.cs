using Flee;
using Flee.PublicTypes;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using snmpMon.Core.Services;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Extensions;
using snmpMon.Helper;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Extenstions;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using snmpMon.Models;
using snmpMon.Services.DeviceServices;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
namespace snmpMon.Services.SensorServices
{
    public class DashboardSensors : IDisposable, IDashboardSensors
    {

        public event EventHandler<DeviceStatusEventArgs> OnDeviceStatusChanged;
        public event EventHandler<ResourceUsageEventArgs> OnResourceDeviceUsage;
        public event EventHandler<InterfaceStatusEventArgs> OnInterfaceStatusChanged; 
        public event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        public event Action<TimeSpan> OnUpTimeReceived;
        private string _ipAddress;
        private string _communityString;
        private string _deviceCategory;
        private CancellationTokenSource _cts;
        private bool _isMonitoring = false;
        private Task _upTimeTask;

        private readonly IDeviceDashboardFacade _deviceFacade;
        public DashboardSensors(IDeviceDashboardFacade deviceFacade)
        {
            _cts = new CancellationTokenSource();
            _deviceFacade = deviceFacade;
            RegisterEvents();
            
        }

        private bool ConvertStatusToBool(int status)
        {
            return status != 1;
        }
        private void RegisterEvents()
        {
            _deviceFacade.OnDeviceStatusChanged += (s, e) => OnDeviceStatusChanged?.Invoke(s, e);
            _deviceFacade.OnResourceDeviceUsage += (s, e) => OnResourceDeviceUsage?.Invoke(s, e);
            _deviceFacade.OnDeviceInterfaceReceived += (list) => OnDeviceInterfaceReceived?.Invoke(list);
            _deviceFacade.OnInterfaceStatusChanged += _deviceFacade_OnInterfaceStatusChanged;
            _deviceFacade.OnUpTimeReceived += (span) => OnUpTimeReceived?.Invoke(span);
        }

        private void _deviceFacade_OnInterfaceStatusChanged(object sender, InterfaceStatusEventArgs e)
        {
            OnInterfaceStatusChanged?.Invoke(this, e);
        }

        private (string MemoryUsage, string cpuTotal1Min) FetchUtilizationNodes()
        {
            var deviceResources = _deviceFacade.FetchUtilizationNodes(_ipAddress, _communityString, _deviceCategory);
            return (deviceResources.MemoryUsage, deviceResources.cpuTotal1Min);

        }

        private void GetDeviceAuthInformation(string ipAddress, string communityString, string deviceCategory)
        {
            _ipAddress = ipAddress;
            _communityString = communityString;
            _deviceCategory = deviceCategory;
        }
        private bool isDeviceUnderMonitoring()
        {
            return _isMonitoring;
        }

        private (IPEndPoint ip, OctetString communityString) GetConvertedInformation()
        {
            return (new IPEndPoint(IPAddress.Parse(_ipAddress), 161), new OctetString(_communityString));
        }

        private void DebugDesoverProblem(string ipAddress)
        {
            Debug.WriteLine("################################################");
            Debug.WriteLine($"********** [{ipAddress}]**********************");
            Debug.WriteLine("################################################");

        }

        public void StartMonitoring(string ipAddress, string communityString, string deviceCategory)
        {
            if (isDeviceUnderMonitoring()) return;
            if(_cts == null || _cts.IsCancellationRequested)
            {
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
            }
            _isMonitoring = true;
            GetDeviceAuthInformation(ipAddress, communityString, deviceCategory);
            int failRepetation = 0;
            var deviceRelatedInfo = GetConvertedInformation();
            
            Task.Run(async () =>
            {
                var utilizationNodes = FetchUtilizationNodes();


                try
                {


                    _= _deviceFacade.StartTrackingAsync(ipAddress, communityString, deviceCategory, _cts.Token);

                    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
                    do
                    {
                        if (await pingAsync())
                        {
                            failRepetation = 0;
                            try
                            {
                                await _deviceFacade.GetResourcesAsync(deviceRelatedInfo.ip, deviceRelatedInfo.communityString, utilizationNodes.MemoryUsage, utilizationNodes.cpuTotal1Min);

                            }
                            catch (Exception ex)
                            {
                                Engine.Helper.Logger.LogCriticalException(ex, ex.Message);
                            }


                        }
                        else
                        {
                            failRepetation++;
                            if (failRepetation >= 3)
                            {
                                _cts?.Cancel();

                                _ = _deviceFacade.StartDeviceChecker(_ipAddress, communityString);
                                OnDeviceStatusChanged?.Invoke(this, new DeviceStatusEventArgs(_ipAddress, "", Enums.DeviceStatus.Down));
                                break;
                            }
                        }

                    } while (await timer.WaitForNextTickAsync(_cts.Token));

                }
                catch (TaskCanceledException) { }

                catch (Exception ex)
                {
                    Debug.WriteLine($"Monitoring Core Error : {ex}");
                }
                finally
                {
                    _isMonitoring = false;
                }
            }, _cts.Token);

        }





        private async Task<bool> pingAsync()
        {
            bool isSuccess = false;
            try
            {
                using (Ping ping = new Ping())
                {
                    var _ping = await ping.SendPingAsync(_ipAddress, 3000);
                    isSuccess = _ping.Status == IPStatus.Success;
                }
            }
            catch (PingException pingEx)
            {
                Engine.Helper.Logger.LogCriticalException(pingEx, pingEx.Message);
            }
            catch (Exception ex)
            {
                Engine.Helper.Logger.LogCriticalException(ex, ex.Message);

            }

            return isSuccess;
        }

        public void Dispose()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _deviceFacade.StopSnmpListener();
        }
    }
}
