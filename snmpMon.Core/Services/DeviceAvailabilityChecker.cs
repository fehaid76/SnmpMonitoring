using Lextm.SharpSnmpLib;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.MibJson.Extenstions;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace snmpMon.Core.Services
{
    public class DeviceAvailabilityChecker : IDeviceAvailabilityChecker
    {
        public event Action<string, string> DevicePowerRestored;
        private CancellationTokenSource _cancellationTokenSource;
        private string _iPAddress;
        private string _communityString;
        private MibNode _sysDescNode;
        private readonly SemaphoreSlim _lock; 

        private readonly ISnmpService _snmpService;
        private readonly IMibParser _mibParser;
        public DeviceAvailabilityChecker(ISnmpService snmpService, IMibParser mibParser)
        {
            _snmpService = snmpService;
            _mibParser = mibParser;
            _lock = new SemaphoreSlim(1, 1);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async Task<MibNode> sysNode()
        {
            var node = await _mibParser.GetNode("sysDescr");
            return node;
        }
        public async Task StartDeviceChecker(string ipAddress, string communityString)
        {
            _iPAddress = ipAddress;
            _communityString = communityString;
            _sysDescNode = await sysNode();
            TimeSpan timeoutDuration = TimeSpan.FromSeconds(3);
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            try
            {
                do
                {
                    try
                    {
                        if (!await _lock.WaitAsync(0))
                            continue;

                        bool pingSuccess = await SendPingAsync();
                        if (pingSuccess)
                        {
                            if (await SendSnmpRequest(timeoutDuration))
                            {
                                DevicePowerRestored?.Invoke(ipAddress, communityString);
                                return;
                            }

                        }
                    }
                    finally
                    {
                        _lock.Release();
                    }



                } while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token));


            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }
        }

        private MibNode GetSnmpSysDescrNode()
        {
            var sysDescrNode = _sysDescNode;
            if (sysDescrNode != null)
                return sysDescrNode;
            return new MibNode();



        }
        private async Task<bool> SendPingAsync()
        {
            using var ping = new Ping();
            try
            {
                var pingResult = await ping.SendPingAsync(_iPAddress, 1000);
                return pingResult.Status == IPStatus.Success;
                    
            }

            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
                return false;
            }

            

        }
        private async Task<bool> SendSnmpRequest(TimeSpan timeoutDuration)
        {
            try
            {
                var sysDescOid = GetSnmpSysDescrNode();
                var snmpResponse = await _snmpService.GetStringAsync(_iPAddress, _communityString, sysDescOid.Oid.ToScalarOid()).WaitAsync(timeoutDuration);
                return (snmpResponse != null);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
                return false;
            }

            

        }

    }
}
