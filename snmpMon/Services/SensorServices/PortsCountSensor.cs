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
    public class PortsCountSensor : ISensor<string, int>
    {
        public bool isRunning { get; set; }
        private CancellationTokenSource _cts;

        public event Action<string, int> OnSensorExecuted;

        IMibParser _mibParser;
        ISnmpService _snmpService;

        public PortsCountSensor(IMibParser mibParser, ISnmpService snmpService)
        {
            _mibParser = mibParser;
            _snmpService = snmpService;
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
                var oid = await _mibParser.GetNode("ifNumber");
                var objectOid = new ObjectIdentifier(oid.Oid.ToScalarOid());
                int currentTimeTick = 3; 
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(currentTimeTick));



                while (!cts.IsCancellationRequested || !_cts.IsCancellationRequested)
                {
                    
                    try
                    {

                        var portsResult = await _snmpService.GetIntAsyn(ip, comString, objectOid);

                        OnSensorExecuted?.Invoke(ipAddress, portsResult);
                        await Task.Delay(TimeSpan.FromSeconds(60));
                        

                    }
                    catch (Lextm.SharpSnmpLib.Messaging.TimeoutException timeOutEx)
                    {
                        Logger.LogCriticalException(timeOutEx, timeOutEx.Message);
                    }
                    catch (SocketException socketEx)
                    {
                        Logger.LogCriticalException(socketEx, socketEx.InnerException.Message);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
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
