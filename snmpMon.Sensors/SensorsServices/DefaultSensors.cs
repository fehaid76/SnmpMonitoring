using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace snmpMon.Sensors.SensorsServices
{
    public class DefaultSensors : IDefaultSensors
    {


        public async Task<bool> PingSensorAsync(string iPAddress)
        {
            bool isSucc = false;

            try
            {
                using (Ping ping = new Ping())
                {
                    var result = await ping.SendPingAsync(iPAddress);
                    if (result.Status == IPStatus.Success)
                    {
                        isSucc = true;
                    }
                }
            }
            catch (PingException ex)
            {
                EventLog eventLog = new EventLog();
                eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
            return isSucc;
        }
        public async Task<string> GetDeviceName(string ipAddress, string communityString)
        {
            string _deviceName = "nEW dEVICE";
            //var oid = new ObjectIdentifier(".1.3.6.1.2.1.1.5.0");
            //List<Variable> vcl = new List<Variable>() { new Variable(oid) };

            //try
            //{
            //    var result = await Messenger.GetAsync(VersionCode.V2,
            //        new IPEndPoint(IPAddress.Parse(ipAddress),
            //        161), new OctetString(communityString), vcl);

            //    if (result != null && result.Count > 0)
            //    {
            //        foreach (var v in result)
            //        {
            //            if (v.Data is OctetString octet)
            //            {
            //                _deviceName = octet.ToString();
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}

            return _deviceName;
        }
        public async Task<int> GetPortsCountstring(string iPAddress, string communityString)
        {
            int _portsNum = 0;
            var oid = new ObjectIdentifier(".1.3.6.1.2.1.2.1.0");
            List<Variable> vcl = new List<Variable>() { new Variable(oid) };

            try
            {
                var result = await Messenger.GetAsync(VersionCode.V2,
                    new IPEndPoint(IPAddress.Parse(iPAddress), 161),
                    new OctetString(communityString), vcl);

                if (result != null && result.Count > 0)
                {
                    if (result[0].Data is Integer32 snmpInteger)
                    {
                        _portsNum = snmpInteger.ToInt32();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return _portsNum;

        }
    }
}
