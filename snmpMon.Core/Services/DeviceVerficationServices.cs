using Lextm.SharpSnmpLib;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Core.Services
{
    public class DeviceVerficationServices : IDeviceVerficationServices
    {
        public event Action<bool> OnPingSuccess;
        public event Action<bool> OnSnmpProtocolSuccess;
        public event Action<string> OnDeviceVendorRecieved;
        public event Action<bool> OnSnmpEnabled; 
        private bool isSnmpEnabled; 
        private string _ipAddress;
        private string _communityString;
        private MibNode? _sysNode;
        private Dictionary<string, string> _sysVendors;

        private readonly ISnmpService _snmpService;
        public DeviceVerficationServices()
        {
            _sysVendors = new Dictionary<string, string>();
            _snmpService = new SnmpService();
            _sysSystemsVendor();
        }

        public async Task StartVerificationDevice(string ipAddress, string communityString, MibNode node)
        {
            _ipAddress = ipAddress;
            _communityString = communityString;
            _sysNode = node;

            try
            {
                var isSuccess = await PingVerification();
                if (isSuccess)
                {
                    var sysOid = await GetOidDevice(_sysNode.Oid);
                    SnmpProtocolCheck(sysOid);
                    GetSystemVendor(sysOid);


                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private async Task<bool> PingVerification(int timeout = 4000)
        {
            var cancellation = new CancellationTokenSource();
            var token = cancellation.Token;
            bool isSuccess = false;
            try
            {
                using (Ping ping = new Ping())
                {
                    var sendPing = await ping.SendPingAsync(IPAddress.Parse(_ipAddress), TimeSpan.FromMicroseconds(timeout), null, null, token);
                    var result = sendPing.Status == IPStatus.Success;
                    if (result)
                    {
                        OnPingSuccess?.Invoke(result);
                        isSuccess = result;


                    }
                    else
                    {
                        OnPingSuccess?.Invoke(false);
                        OnSnmpProtocolSuccess?.Invoke(false);
                        OnDeviceVendorRecieved?.Invoke(string.Empty);
                        cancellation.Cancel();
                        cancellation.Dispose();
                        return false;
                    }
                }
            }
            catch (PingException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return isSuccess;

        }
        private void SnmpProtocolCheck(string sysOid)
        {

            if (!string.IsNullOrEmpty(sysOid))
                OnSnmpEnabled?.Invoke(true);
            else
                OnSnmpEnabled?.Invoke(false);
                

        }
        private void GetSystemVendor(string? sysOID)
        {
            string sysVendor = string.Empty;
            string[] sysOidSub;
            string vendorOid = string.Empty;
            if (!string.IsNullOrEmpty(sysOID))
            {
                sysOidSub = sysOID.Split('.');
                vendorOid = sysOidSub[6];

            }
            if (_sysVendors.TryGetValue(vendorOid, out string? sysV))
            {
                
                sysVendor = sysV;
                OnDeviceVendorRecieved?.Invoke(sysVendor);
            }
            else
            {
                OnDeviceVendorRecieved?.Invoke(string.Empty);

            }
        }
        private void _sysSystemsVendor()
        {
            _sysVendors.Add("9", "Cisco");
            _sysVendors.Add("11", "HP");
            _sysVendors.Add("2011", "Huawei");
            _sysVendors.Add("2636", "Juniper");

        }
        private async Task<string> GetOidDevice(string oid)
        {
            try
            {
                var walkNode = await _snmpService.GetWalkAsync(_ipAddress, _communityString, oid);
                if (walkNode.Count > 0)
                {
                    OnSnmpProtocolSuccess?.Invoke(true);
                    return walkNode[0].Data.ToString();
                }
                else
                {
                    OnSnmpProtocolSuccess?.Invoke(false);
                    return string.Empty;
                }


            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch (Exception ex)
            {
                return string.Empty;

            }

            return string.Empty;
        }
    }
}
