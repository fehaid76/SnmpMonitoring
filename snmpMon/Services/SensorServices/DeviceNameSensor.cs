using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.MibJson.Extenstions;
using snmpMon.MibJson.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.SensorServices
{
    public class DeviceNameSensor : ISensor<string, string>
    {
        public bool isRunning { get; set; }
        private CancellationTokenSource _cts;

        public event Action<string, string> OnSensorExecuted;

        private readonly ISnmpService _snmpService;
        private readonly IMibParser _mibParser;
        public DeviceNameSensor(ISnmpService snmpService, IMibParser mibParser)
        {
            _snmpService = snmpService;
            _mibParser = mibParser;
            isRunning = false;
            _cts = new CancellationTokenSource();


        }

        private (IPEndPoint ipAddress, OctetString communityString) GetDeviceAuthInformation(string ipAddress, string communityString)
        {
            return (new IPEndPoint(IPAddress.Parse(ipAddress), 161), new OctetString(communityString));
        }

        private async Task<ObjectIdentifier> GetSysNameOid()
        {
            var oid = await _mibParser.GetNode("sysName");
            var objectID = new ObjectIdentifier(oid.Oid.ToScalarOid());
            return objectID;
        }

        public async Task PollAsync(string ipAddress, string communityString, CancellationToken cts)
        {
            if (isRunning) return;
            isRunning = true;


            var deviceAuthDetails = GetDeviceAuthInformation(ipAddress, communityString);
            var objectID = await GetSysNameOid();

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

            do
            {
                try
                {
                    if (cts.IsCancellationRequested || this._cts.IsCancellationRequested)
                        break;

                    var _deviceNameResult = await _snmpService.GetStringAsync(deviceAuthDetails.ipAddress, deviceAuthDetails.communityString, objectID);
                    OnSensorExecuted?.Invoke(ipAddress, _deviceNameResult);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.LogCriticalException(ex, ex.Message);
                }
                finally
                {
                    isRunning = false;
                }

            } while (await timer.WaitForNextTickAsync());

        }

        public void Dispose()
        {
            _cts?.Cancel();
        }
    }
}
