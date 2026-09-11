using Flee;
using Flee.PublicTypes;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using snmpMon.Core.Services;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Extensions;
using snmpMon.Helper;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Extenstions;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
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
using System.Windows.Threading;
namespace snmpMon.Services.SensorServices
{
    public class DashboardSensors : IDisposable, IDashboardSensors
    {

        public event EventHandler<DeviceStatusEventArgs> OnDeviceStatusChanged;
        public event EventHandler<ResourceUsageEventArgs> OnResourceDeviceUsage;

        public event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        public event Action<string, int> OnInterfaceStatusChanged;
        public event Action<TimeSpan> OnUpTimeReceived;
        public event Action<SystemInformation> OnSystemInformationReceived;
        private List<DeviceInterface> _interfaces = new();
        private string _ipAddress;
        private string _communityString;
        private string _deviceCategory;
        private int _calculatingMode;
        private CancellationTokenSource _cts;
        private bool _isMonitoring = false;
        private Task _upTimeTask;

        private readonly ExpressionContext _context = new();
        private readonly DynamicExpresso.Interpreter _interpreter = new();

        private readonly ISnmpListener _snmpListener;
        private readonly ISnmpService _snmpService;
        private readonly IMibParser _mibParser;
        private readonly IDeviceAvailabilityChecker _deviceAvailability;
        public DashboardSensors(ISnmpListener snmpListener, ISnmpService snmpService, IMibParser mibParser, IDeviceAvailabilityChecker deviceAvailability)
        {
            _cts = new CancellationTokenSource();
            _snmpListener = snmpListener;
            _snmpService = snmpService;
            _mibParser = mibParser;
            _deviceAvailability = deviceAvailability;

            _deviceAvailability.DevicePowerRestored += _deviceAvailability_DevicePowerRestored;
            _snmpListener.OnInterfaceStatusChanged += _snmpListener_OnInterfaceStatusChanged;
        }

        private void _deviceAvailability_DevicePowerRestored(string arg1, string arg2)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_cts.IsCancellationRequested)
                    _cts = new CancellationTokenSource();

                OnDeviceStatusChanged?.Invoke(this, new DeviceStatusEventArgs(arg1, arg2, Enums.DeviceStatus.Up));
                _isMonitoring = true;
            });

        }



        private (string MemoryUsage, ObjectIdentifier cpuTotal1Min) FetchUtilizationNodes()
        {
            try
            {
                var deviceNode = _mibParser.GetDeviceNode(_deviceCategory);

                if (deviceNode?.Resources == null)
                {
                    throw new InvalidOperationException("Device node or resources configuration is null.");
                }

                var memoryOid = deviceNode.Resources.MemoryConfig?.MemoryUsageOID;
                var cpuOid = deviceNode.Resources.CpuUsage;

                return (
                    string.IsNullOrEmpty(memoryOid) ? string.Empty : memoryOid,
                    string.IsNullOrEmpty(cpuOid) ? new ObjectIdentifier(string.Empty) : new ObjectIdentifier(cpuOid)
                );
            }
            catch (Exception ex)
            {
                Engine.Helper.Logger.LogCriticalException(ex, ex.Message);
                return (string.Empty, new ObjectIdentifier(string.Empty));
            }
        }

        public void StartMonitoring(string ipAddress, string communityString, string deviceCategory)
        {
            if (_isMonitoring) return;
            _isMonitoring = true;

            _ipAddress = ipAddress;
            _communityString = communityString;
            _deviceCategory = deviceCategory;
            int failRepetation = 0;
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(_ipAddress), 161);
            OctetString community = new OctetString(communityString);

            Task.Run(async () =>
            {
                var utilizationNodes = FetchUtilizationNodes();
                var nodeChecker = await _mibParser.GetNode("sysDescr");
                var sysNode = await _mibParser.GetNode("system");

                try
                {
                    _ = GetUpTime();
                    await GetInterfacesAsync();
                    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
                    do
                    {
                        if (await pingAsync())
                        {
                            failRepetation = 0;
                            try
                            {
                                var systemNameTask = GetSystemName(ip, community, new ObjectIdentifier(sysNode.Oid.ToString()));
                                var resourceUsageTask = GetResourceUsage(ip, community, utilizationNodes.MemoryUsage, utilizationNodes.cpuTotal1Min);
                                await Task.WhenAll(systemNameTask, resourceUsageTask);

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

                                _ = _deviceAvailability.StartDeviceChecker(_ipAddress, communityString, nodeChecker);
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

        private double EvaluateFormal(string formal)
        {
            var expression = formal.ReplaceSpaces().Trim('{', '}').AddDecimalToInt();
            var result = _interpreter.Eval(expression);
           

            return Convert.ToDouble(result);
        }
        public async Task GetUpTime()
        {
            if (_upTimeTask != null && !_upTimeTask.IsCompleted)
                return;

            var task = RunUpTimeLoopAsync();
            _upTimeTask = task;

            await _upTimeTask;



        }
        private async Task RunUpTimeLoopAsync()
        {
            var upTimeNode = await _mibParser.GetNode("sysUpTime");
            var upTime = await _snmpService.GetUpTime(_ipAddress, _communityString, upTimeNode.Oid.ToScalarOid());
            OnUpTimeReceived?.Invoke(upTime);

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            try
            {
                while (await timer.WaitForNextTickAsync())
                {
                    upTime += TimeSpan.FromSeconds(1);
                    OnUpTimeReceived?.Invoke(upTime);
                    if (_cts.IsCancellationRequested)
                        break;
                }

            }
            catch (Exception ex)
            {
                Engine.Helper.Logger.LogCriticalException(ex, ex.Message);
            }
        }


        private async Task<(MibNode Desc, MibNode Speed, MibNode Admin, MibNode Oper)> fetchInterfaceOidsAsync()
        {
            var descTask = _mibParser.GetNode("ifDescr");
            var speedTask = _mibParser.GetNode("ifSpeed");
            var adminTask = _mibParser.GetNode("ifAdminStatus");
            var operTask = _mibParser.GetNode("ifOperStatus");

            await Task.WhenAll(descTask, speedTask, adminTask, operTask);

            return (descTask.Result, speedTask.Result, adminTask.Result, operTask.Result);
        }

        private async Task<(IList<SnmpVariable> Desc, IList<SnmpVariable> Speed, IList<SnmpVariable> Admin, IList<SnmpVariable> Oper)> fetchInterfaceDataSync((MibNode Desc, MibNode Speed,
            MibNode Admin, MibNode Oper) Oids)
        {
            var descTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Desc.Oid);
            var speedTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Speed.Oid);
            var adminTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Admin.Oid);
            var operTask = _snmpService.GetBulkAsync(_ipAddress, _communityString, Oids.Oper.Oid);

            await Task.WhenAll(descTask, speedTask, adminTask, operTask);

            return (descTask.Result, speedTask.Result, adminTask.Result, operTask.Result);

        }

        private IEnumerable<DeviceInterface> MapToDevicceInterfaces((IList<SnmpVariable> Desc, IList<SnmpVariable> Speed, IList<SnmpVariable> Admin, IList<SnmpVariable> Oper) rawData)
        {
            var speedDic = rawData.Speed.ToDictionary(s => s.Oid.GetOidIndex(), s => GetParsedValue<long>(s.Data));
            var adminDic = rawData.Admin.ToDictionary(a => a.Oid.GetOidIndex(), a => GetParsedValue<int>(a.Data));
            var operDic = rawData.Oper.ToDictionary(o => o.Oid.GetOidIndex(), o => GetParsedValue<int>(o.Data));

            foreach (var desc in rawData.Desc)
            {
                var index = desc.Oid.GetOidIndex();

                yield return new DeviceInterface()
                {
                    Description = desc.Data.ToString(),
                    IfIndex = index,
                    Speed = speedDic.GetValueOrDefault(index),
                    AdminStatus = adminDic.GetValueOrDefault(index),
                    OperationStatus = operDic.GetValueOrDefault(index)
                };
            }
        }

        private T GetParsedValue<T>(object value) where T : struct
        {
            if (value != null && Convert.ChangeType(value.ToString(), typeof(T)) is T result)
                return result;
            return default;

        }

        private async Task GetInterfacesAsync()
        {
            var oids = await fetchInterfaceOidsAsync();
            var rawResults = await fetchInterfaceDataSync(oids);

            _interfaces.Clear();

            _interfaces.AddRange(MapToDevicceInterfaces(rawResults));

            OnDeviceInterfaceReceived?.Invoke(_interfaces);
        }

        private void _snmpListener_OnInterfaceStatusChanged(string arg1, bool arg2)
        {


            int interfaceStatus = arg2 ? 1 : 2;
            OnInterfaceStatusChanged?.Invoke(arg1, interfaceStatus);
        }

        private async Task<double> CalculateOidsString(string value, IPEndPoint ip, OctetString Community)
        {
            var group = value.ConvertToOidList();
            var sd = new StringBuilder(value);
            var resultGroup = await _snmpService.GetGroupDoubleAsync(_ipAddress, _communityString, group);
            foreach (var o in resultGroup)
            {
                sd.Replace(o.oid, o.result.ToString());
            }
            return EvaluateFormal(sd.ToString());
        }

        private async Task<double> GetIntAsyc(IPEndPoint ip, OctetString Community, string oid)
        {
            return await _snmpService.GetIntAsync(ip.ToString(), Community.ToString(), oid);
        }

        private async Task<double> FetchMemoryUsage(IPEndPoint ip, OctetString Community, string MemoryOid)
        {
            Task<double> MemoryTask = null;

            if (MemoryOid.ToString().GetOidsCount() > 1)
            {
                 MemoryTask =  CalculateOidsString(MemoryOid.ToString(), ip, Community);
            }
            else
            {
                 MemoryTask = _snmpService.GetDoubleAsync(ip, Community, new ObjectIdentifier(MemoryOid));
  
            }
            
            return await MemoryTask;

            // var MemoryTask = _snmpService.GetIntAsyn(ip, Community, MemoryOid);

            //await MemoryTask;

            //return MemoryTask;
        }
        private async Task<double> GetMemeoryUsageAsync(IPEndPoint ip, OctetString Community, string MemoryOid)
        {

            var memoryUsageDetails = await FetchMemoryUsage(ip, Community, MemoryOid);
            return Math.Round(memoryUsageDetails, 2);
            //if ((memoryUsageDetails.memoryUsed + memoryUsageDetails.memoryFree) > 0)
            //{
            //    var _memoryUsage = (memoryUsageDetails.memoryUsed / (memoryUsageDetails.memoryUsed + memoryUsageDetails.memoryFree) * 100);
            //    return Math.Round(_memoryUsage, 2);
            //}

            return 0;
        }

        private async Task<int> GetCPUtilizationAsync(IPEndPoint ip, OctetString Community, ObjectIdentifier cpuUtilizationOid)
        {

            int cpuUtilization = await _snmpService.GetIntAsyn(ip, Community, cpuUtilizationOid);
            return cpuUtilization;
        }

        private async Task GetResourceUsage(IPEndPoint ip, OctetString communityString, string MemoryUsageOid, ObjectIdentifier cpuUtilizationOid)
        {
            var memoryUsageTask = GetMemeoryUsageAsync(ip, communityString, MemoryUsageOid);
            var cpuUtilization = GetCPUtilizationAsync(ip, communityString, cpuUtilizationOid);

            await Task.WhenAll(memoryUsageTask, cpuUtilization);

            OnResourceDeviceUsage?.Invoke(this, new ResourceUsageEventArgs(memoryUsageTask.Result, cpuUtilization.Result));
        }



        private async Task GetSystemName(IPEndPoint ip, OctetString Community, ObjectIdentifier sysOid)
        {
            var systemInformation = await _snmpService.GetBulkAsync(ip, Community, sysOid);

            if (systemInformation != null)
            {
                Debug.WriteLine($"## {systemInformation.Count} ##");
            }

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

            _snmpListener.StopSnmpListener();
        }
    }
}
