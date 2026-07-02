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
            
            if (!isRunning)
            {
                isRunning = true;
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var pingEcho = await PingAsync(ipAddress);
                        OnSensorExecuted?.Invoke(ipAddress, pingEcho);
                        await Task.Delay(TimeSpan.FromSeconds(10), _cts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
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

        private async Task<bool> PingAsync(string ipAddress)
        {
            

            try
            {

                using (Ping ping = new Ping())
                {
                    var result = await ping.SendPingAsync(ipAddress, 4000);
                    return result.Status == IPStatus.Success;
                }
            }
            catch (PingException ex)
            {
                Debug.WriteLine($"this is ping exception => {ex.Message}");
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
