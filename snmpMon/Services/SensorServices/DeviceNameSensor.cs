using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
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
        public CancellationTokenSource _cts { get; set; }

        public event Action<string, string> OnSensorExecuted;

        public DeviceNameSensor()
        {
            isRunning = false;
            _cts = new CancellationTokenSource();
        }

        public async Task PollAsync(string ipAddress, string communityString)
        {
            if (!isRunning)
            {
                while (!_cts.IsCancellationRequested)
                {
                    isRunning = true;
                    try
                    {
                        var _deviceNameResult = await _getDeviceNameAsync(ipAddress, communityString, _cts.Token);
                        OnSensorExecuted?.Invoke(ipAddress, _deviceNameResult);
                        await Task.Delay(TimeSpan.FromSeconds(10), _cts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;

                    }
                    catch (System.Net.Sockets.SocketException) { }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        isRunning = false;
                    }

                }
            }

            
            
        }

        public async Task<string> _getDeviceNameAsync(string ipAddress, string communityString, CancellationToken token)
        {
            string _deviceName = string.Empty;
            var _oid = new ObjectIdentifier(".1.3.6.1.2.1.1.5.0");
            List<Variable> _vcl = new List<Variable>() { new Variable(_oid) };

            try
            {
                var result = await Messenger.GetAsync(VersionCode.V2,
                   new IPEndPoint(IPAddress.Parse(ipAddress), 161),
                   new OctetString(communityString), _vcl);

                if (result != null && result.Count > 0)
                {
                    foreach (var v in result)
                    {
                        if(v.Data is OctetString octect)
                        {
                            _deviceName = octect.ToString();
                        }
                    }
                }
            }
            catch (SocketException ex) { }
            catch (Exception e)
            {

            }

            return _deviceName;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
