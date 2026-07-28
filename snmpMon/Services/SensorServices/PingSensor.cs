using snmpMon.Engine.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.SensorServices
{
    public class PingSensor : ISensor<string, bool>
    {

        public bool isRunning { get; set; }
        public CancellationTokenSource _cts { get; set; }

        public PingSensor()
        {
            _cts = new CancellationTokenSource();
            isRunning = false;
        }

        public event Action<string, bool> OnSensorExecuted;

        public async Task PollAsync(string ipAddress, string communityString)
        {
            if (isRunning) return;
            isRunning = true;

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var pingEcho = await PingAsync(ipAddress);
                        OnSensorExecuted?.Invoke(ipAddress, pingEcho);
                        await Task.Delay(TimeSpan.FromSeconds(10), _cts.Token);
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

        private async Task<bool> PingAsync(string ipAddress, int timeout = 4000)
        {
            
            

            try
            {

                using (Ping ping = new Ping())
                {
                    var result = await ping.SendPingAsync(ipAddress, timeout);
                    return result.Status == IPStatus.Success;
                }
            }
            catch (PingException ex)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
