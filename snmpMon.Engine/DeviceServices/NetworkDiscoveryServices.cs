using snmpMon.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.DeviceServices
{
    public class NetworkDiscoveryServices
    {
        public IPAddressModel GetMyNetworkDetails()
        {
            IPAddressModel iPAddressModel = new IPAddressModel();
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus == OperationalStatus.Up &&
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                    || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                    {
                        iPAddressModel.IPAddress = ip.Address.ToString();
                        iPAddressModel.Subnetmask = ip.IPv4Mask.ToString();
                    }
                }
            }
            return iPAddressModel;
        }
        public async Task<List<string>> GetAllActiveAddresses(IPAddressModel Subnetmask)
        {
            List<string> _ActiveIPs = new List<string>();
            List<Task> _tasks = new List<Task>();

            SemaphoreSlim semaphore = new SemaphoreSlim(500);
            int totalHosts = UtilityCalculating.GetHostsCountInSubnet(Subnetmask.Subnetmask);
            for (int i = 0; i <= totalHosts; i++)
            {
                string ip = UtilityCalculating.CalculateIpAddresses(Subnetmask.Subnetmask, i);
                await semaphore.WaitAsync();
                string _ip = Subnetmask.IPAddress;
                var task = Task.Run(async () =>
                {
                    try
                    {
                        using (Ping ping = new Ping())
                        {
                            PingReply replay = await ping.SendPingAsync(ip);
                            if (replay.Status == IPStatus.Success)
                            {
                                lock (_ActiveIPs)
                                {
                                    _ActiveIPs.Add(ip);
                                }
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                _tasks.Add(task);
            }

            await Task.WhenAll(_tasks);
            return _ActiveIPs;



        }

    }
}
