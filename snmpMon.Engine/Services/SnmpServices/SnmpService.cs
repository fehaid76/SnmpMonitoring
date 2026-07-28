using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using snmpMon.Engine.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpServices
{
    public class SnmpService : ISnmpService
    {
        
        public async Task<int> GetIntAsyn(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid)
        {
            int snmpValue = 0; 

            List<Variable> vcl = new List<Variable>(1) { new Variable(oid) };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                var result = await Messenger.GetAsync(VersionCode.V2,
                    ip, communityString,
                    vcl, cts.Token);

                if (result != null && result.Count > 0)
                {
                    if (result[0].Data is Gauge32 snmpGauge)
                    {
                        snmpValue = (int)snmpGauge.ToUInt32();
                    }
                    else if (result[0].Data is Integer32 intValue)
                    {
                        snmpValue = intValue.ToInt32();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch(System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch(Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return snmpValue;
        }
        public async Task<int> GetIntAsync(string ip, string communityString, string oid, int port = 161)
        {
            return await GetIntAsyn(new IPEndPoint(IPAddress.Parse(ip), 161), new OctetString(communityString), new ObjectIdentifier(oid));
        }
        public async Task<string> GetStringAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid)
        {

            List<Variable> vcl = new List<Variable>(1) { new Variable(oid) };

            try
            {
                var reault = await Messenger.GetAsync(VersionCode.V2, ip, communityString, vcl);

                if (reault != null && reault.Count > 0)
                {
                    if (reault[0].Data is OctetString octet)
                    {
                        return octet.ToString();
                    }
                }
            }
            catch (SocketException ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return string.Empty;
        }
        public async Task<string> GetStringAsync(string ip, string communityString, string oid, int port = 161)
        {
            return await GetStringAsync(new IPEndPoint(IPAddress.Parse(ip), port), new OctetString(communityString), new ObjectIdentifier(oid));
        }
        public async Task<TimeSpan> GetUpTime(string ip, string communityString, string oid)
        {
            TimeSpan timeSpan = new TimeSpan();
            var _oid = new ObjectIdentifier(oid);
            List<Variable> vcl = new List<Variable>() { new Variable(_oid) };

            try
            {
                var result = await Messenger.GetAsync(VersionCode.V2,
                    new IPEndPoint(IPAddress.Parse(ip), 161), new OctetString(communityString), vcl);

                if (result != null && result.Count > 0)
                {
                    if (result[0].Data is TimeTicks upTime)
                    {
                        timeSpan = (TimeSpan)upTime.ToTimeSpan();
                    }
                }
            }
            catch (Lextm.SharpSnmpLib.Messaging.TimeoutException tx)
            {
                Logger.LogCriticalException(tx, tx.Message);
            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, "");
            }

            return timeSpan;
        }
        public async Task<List<Variable>> GetWalkAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid)
        {
            var internalList = new List<Variable>();

            try
            {

                await Messenger.WalkAsync(VersionCode.V2,
                ip,
                communityString, oid,
                internalList, WalkMode.WithinSubtree);
                return internalList;
            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch (Lextm.SharpSnmpLib.Messaging.TimeoutException tx)
            {
                Logger.LogCriticalException(tx, tx.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return new List<Variable>();
        }
        public async Task<List<Variable>> GetWalkAsync(string ip, string communityString, string oid, int port = 161)
        {

            return await GetWalkAsync(new IPEndPoint(IPAddress.Parse(ip), port), new OctetString(communityString), new ObjectIdentifier(oid));
        }
        public async Task<List<SnmpVariable>> GetBulkAsync(string ip, string communityString, string oid, int maxRepetation = 10)
        {
            var internalList = new List<Variable>();
            var _snmpVariables = new List<SnmpVariable>();

            try
            {
                  await Messenger.BulkWalkAsync(VersionCode.V2,
                    new IPEndPoint(IPAddress.Parse(ip), 161),
                    new OctetString(communityString),
                    OctetString.Empty,
                    new ObjectIdentifier(oid),
                    internalList,
                    maxRepetation,
                    WalkMode.WithinSubtree,
                    null,
                    null);

                SnmpVariable snmpVariable;
                foreach (var v in internalList)
                {
                    snmpVariable = new SnmpVariable(v.Id.ToString(), v.Data);
                    _snmpVariables.Add(snmpVariable);
                }

                return _snmpVariables;
                
            }

            catch(Lextm.SharpSnmpLib.Messaging.TimeoutException tx)
            {
                Logger.LogCriticalException(tx, tx.Message);
            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return new List<SnmpVariable>();
        }
    }
}
