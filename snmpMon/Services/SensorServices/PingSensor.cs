using snmpMon.Engine.Helper;
using System;
using System.Buffers;
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
        private readonly byte[] _pingBuffer = new byte[32]; 
        private readonly PingOptions _pingOptions = new PingOptions(64, true);
        private Ping _ping = new Ping();
        public bool isRunning { get; set; }
        private CancellationTokenSource _cts;

        public PingSensor()
        {
            isRunning = false;
            _cts = new CancellationTokenSource();
        }

        public event Action<string, bool> OnSensorExecuted;

        public async Task PollAsync(string ipAddress, string communityString, CancellationToken cts)
        {
            if (isRunning) return;
            isRunning = true;

            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

                do
                {
                    if (_cts.IsCancellationRequested)
                        break;
                    try
                    {
                        var pingEcho = await PingAsync(ipAddress);
                        OnSensorExecuted?.Invoke(ipAddress, pingEcho);

                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCriticalException(ex, ex.Message);
                    }

                } while (await timer.WaitForNextTickAsync(cts));

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

                var result = await _ping.SendPingAsync(ipAddress, timeout, _pingBuffer, _pingOptions);
                return result.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
        }
    }
}
