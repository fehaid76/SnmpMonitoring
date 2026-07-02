using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using snmpMon.Engine.Services.OidServices;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Helper;
using snmpMon.MibJson.Services;
using snmpMon.Models;
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

namespace snmpMon.Services.SensorServices
{
    public class DashboardSensors : IDisposable, IDashboardSensors
    {
        public event Action<double> OnMemoryUsageReceived;
        public event Action<int> OnCPUutilizationReceived;
        public event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        public event Action<string, int> OnInterfaceStatusChanged;
        public event Action<TimeSpan> OnUpTimeReceived;
        public event Action<SystemInformation> OnSystemInformationReceived;
        private List<DeviceInterface> _interfaces = new List<DeviceInterface>();
        private string _ipAddress;
        private string _communityString;
        private CancellationTokenSource _cts;
        private bool _isMonitoring = false;
        private Task _upTimeTask;

        private readonly ISnmpListener _snmpListener;
        private readonly ISnmpService _snmpService;
        private readonly IOidService _oidService;
        private readonly IMibParser _mibParser;
        public DashboardSensors(ISnmpListener snmpListener, ISnmpService snmpService, IOidService oidService, IMibParser mibParser)
        {
            _cts = new CancellationTokenSource();
            _snmpListener = snmpListener;
            _snmpService = snmpService;
            _oidService = oidService;
            _mibParser = mibParser;

            
            _snmpListener.OnInterfaceStatusChanged += _snmpListener_OnInterfaceStatusChanged;
        }

        public void StartMonitoring(string ipAddress, string communityString)
        {
            if (_isMonitoring) return;
            _isMonitoring = true;

            _ipAddress = ipAddress;
            _communityString = communityString;



            Task.Run(async () =>
            {
                try
                {
                    _ = GetUpTime();
                    await GetInterfacesAsync();
                    while (!_cts.IsCancellationRequested)
                    {
                        if (await pingAsync())
                        {
                            try
                            {

                                var memoryTask = GetMemeoryUsageAsync();
                                var cpuTask = GetCPUutilizationAsync();
                                await Task.WhenAll(memoryTask, cpuTask);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogCriticalException(ex, ex.Message);
                            }

                            
                        }

                        await Task.Delay(TimeSpan.FromSeconds(5), _cts.Token);
                    }



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
            });

        }


        public async Task GetUpTime()
        {
            if (_upTimeTask != null && !_upTimeTask.IsCompleted)
                return;

            var upTime = await _snmpService.GetUpTime(_ipAddress, _communityString, _oidService.SysUpTimeOid());
            OnUpTimeReceived?.Invoke(upTime);


            _upTimeTask = Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(1000, _cts.Token);
                        upTime += TimeSpan.FromSeconds(1);
                        OnUpTimeReceived?.Invoke(upTime);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }


                }
            });

            await _upTimeTask;



        }
        private async Task GetInterfacesAsync()
        {
            var descTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, _oidService.InterfacesDescOid());
            var speedTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, _oidService.InterfacesSpeed());
            var adminStatusTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, _mibParser.GetNode("ifAdminStatus").Oid);
            var operStatusTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, _oidService.InterfaceOperStatusOid());

            await Task.WhenAll(descTask, speedTask, adminStatusTask, operStatusTask);

            var _descResult = descTask.Result;
            var _speedResult = speedTask.Result;
            var _adminStatusResult = adminStatusTask.Result;
            var _operStatusResult = operStatusTask.Result;

            _interfaces.Clear();

            var speedDisctionary = _speedResult.ToDictionary(s => GetOidIndex(s.Oid), s => s);
            var adminStatusDic = _adminStatusResult.ToDictionary(a => GetOidIndex(a.Oid), a => a);
            var operStatusDic = _operStatusResult.ToDictionary(o => GetOidIndex(o.Oid), o => o);

            foreach (var i in _descResult)
            {
                var currentIndex = GetOidIndex(i.Oid);

                long speedValue = 0;
                int adminValue = 0;
                int operValue = 0;

                if (speedDisctionary.TryGetValue(currentIndex, out var speedMatch) && 
                    long.TryParse(speedMatch.Data.ToString(), out long parsedSpeed))
                {
                    speedValue = parsedSpeed;
                }
                if (adminStatusDic.TryGetValue(currentIndex, out var adminStatusVal) && 
                    int.TryParse(adminStatusVal.Data.ToString(), out int parsedAdminStatus))
                {
                    adminValue = parsedAdminStatus;
                }
                if (operStatusDic.TryGetValue(currentIndex, out var operStatusVal) && 
                    int.TryParse(operStatusVal.Data.ToString(), out int parsedOperStatus))
                {
                    operValue = parsedOperStatus;
                }

                    _interfaces.Add(new DeviceInterface()
                    {
                        Description = i.Data.ToString(),
                        IfIndex = currentIndex,
                        Speed = speedValue,
                        AdminStatus = adminValue,
                        OperationStatus = operValue,


                });
            }
            OnDeviceInterfaceReceived?.Invoke(_interfaces);
        }     
        private void _snmpListener_OnInterfaceStatusChanged(string arg1, bool arg2)
        {
            Debug.WriteLine("#################################################");
            Debug.WriteLine($"## New SNMP Traps : {arg1} - {arg2} ##");
            Debug.WriteLine("#################################################");

            int interfaceStatus = arg2 ? 1 : 2;
            OnInterfaceStatusChanged?.Invoke(arg1, interfaceStatus);
        }
        private async Task GetMemeoryUsageAsync()
        {


            var usedTask = _snmpService.GetIntAsyn(_ipAddress, _communityString, _oidService.UsedMemoeryOid());
            var freeTask = _snmpService.GetIntAsyn(_ipAddress, _communityString, _oidService.FreeMemoeryOid());
            await Task.WhenAll(usedTask, freeTask);

            double _memoryUsed = await usedTask;
            double _memoryFree = await freeTask;

            if ((_memoryUsed + _memoryFree) > 0)
            {
                var _memoryUsage = (_memoryUsed / (_memoryUsed + _memoryFree) * 100);
               
                OnMemoryUsageReceived?.Invoke((int)Math.Round(_memoryUsage, 2));
            }
        }    
        private async Task GetCPUutilizationAsync()
        {

            int _cpuUtilization = await _snmpService.GetIntAsyn(_ipAddress, _communityString, _oidService.CpuUtilization());
            OnCPUutilizationReceived?.Invoke(_cpuUtilization);
        }
        private string GetOidIndex(string oid)
        {
            return oid.Substring(oid.LastIndexOf('.') + 1);
        }
        private async Task<bool> pingAsync()
        {
            bool isSuccessed = false;
            try
            {
                using (Ping ping = new Ping())
                {
                    var _ping = await ping.SendPingAsync(_ipAddress, 3000);
                    isSuccessed = _ping.Status == IPStatus.Success;
                }
            }
            catch (PingException pingEx)
            {
                Logger.LogCriticalException(pingEx, pingEx.Message);
            }

            return isSuccessed;
        }
        public void Dispose()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _snmpListener.StopSnmpListener();
        }
    }
}
