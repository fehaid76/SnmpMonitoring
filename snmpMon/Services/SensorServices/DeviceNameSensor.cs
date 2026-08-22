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

        public async Task PollAsync(string ipAddress, string communityString, CancellationToken cts)
        {
            if (isRunning) return;
            isRunning = true;
            

                try
                {
                    var ip = new IPEndPoint(IPAddress.Parse(ipAddress), 161);
                    var comString = new OctetString(communityString);
                    var oid = await _mibParser.GetNode("sysName");
                    var objectID = new ObjectIdentifier(oid.Oid.ToScalarOid());

                    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

                    while (!cts.IsCancellationRequested || !this._cts.IsCancellationRequested)
                    {
                        
                        try
                        {
                            var _deviceNameResult = await _snmpService.GetStringAsync(ip, comString, objectID);
                            OnSensorExecuted?.Invoke(ipAddress, _deviceNameResult);
                            await Task.Delay(TimeSpan.FromSeconds(10));
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
                  

                    }

                }
                finally
                {
                    isRunning = false;
                }
          

            
            
        }

        public void Dispose()
        {
            _cts?.Cancel();
        }
    }
}
