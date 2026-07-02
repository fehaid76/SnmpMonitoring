using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using snmpMon.Engine.Helper;
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
        public CancellationTokenSource _cts { get; set; }

        public event Action<string, int> OnSensorExecuted;

        public PortsCountSensor()
        {
            isRunning = false;
            _cts = new CancellationTokenSource();
        }

        public async Task PollAsync(string ipAddress, string communityString)
        {
            if (!isRunning)
            {
                isRunning = true;
                try
                {
                    while (!_cts.IsCancellationRequested)
                    {

                        try
                        {
                            var portsResult = await _portsCountAsync(ipAddress, communityString);
                            OnSensorExecuted?.Invoke(ipAddress, portsResult);

                            await Task.Delay(TimeSpan.FromSeconds(10));
                        }
                        catch (Lextm.SharpSnmpLib.Messaging.TimeoutException timeOutEx)
                        {
                            Logger.LogCriticalException(timeOutEx, timeOutEx.InnerException.Message);
                        }
                        catch (SocketException socketEx)
                        {
                            Logger.LogCriticalException(socketEx, socketEx.InnerException.Message);
                        }
                        catch (TaskCanceledException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }


                    }
                }
                finally
                {
                    isRunning = false;

                }

            }


        }

        private async Task<int> _portsCountAsync(string ipAddress, string communityString)
        {
            int _ports = 0;
            var oid = new ObjectIdentifier(".1.3.6.1.2.1.2.1.0");
            List<Variable> _vcl = new List<Variable>() { new Variable(oid) };

            try
            {
                
                var result = await Messenger.GetAsync(VersionCode.V2,
                    new IPEndPoint(IPAddress.Parse(ipAddress), 161),
                    new OctetString(communityString),
                    _vcl);

                if(result != null && result.Count > 0)
                {
                    if (result[0].Data is Integer32 snmpInteger)
                    {
                        _ports = snmpInteger.ToInt32();
                    }
                }
            }
            catch (Lextm.SharpSnmpLib.Messaging.TimeoutException timeOutEx)
            {
                Logger.LogCriticalException(timeOutEx, timeOutEx.Message);
            }
            catch (SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return _ports; 
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
