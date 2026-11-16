using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using snmpMon.Data.Extensions;
using snmpMon.Engine.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpServices
{
    public class SnmpService : ISnmpService
    {
        private readonly byte[] _pingBuffer = new byte[32];
        private readonly PingOptions _pingOptions = new PingOptions(64, true);

        private async Task<bool> PingAsync(string ipAddress)
        {
            using Ping ping = new();
            try
            {
                var pingEcho = await ping.SendPingAsync(ipAddress, 4000, _pingBuffer, _pingOptions);
                return pingEcho.Status == IPStatus.Success;

            }
            catch(PingException ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
                return false;
            }
        }
        public async Task<double> GetDoubleAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid)
        {
            if (!await PingAsync(ip.Address.ToString()))
                return 0;

            var vcl = new[] { new Variable(oid) };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            try
            {
                var result = await Messenger.GetAsync(VersionCode.V2,
                    ip, communityString, vcl).WaitAsync(cts.Token);
                if(result != null && result?.Count > 0)
                {
                    if (result[0].Data is Gauge32 snmpGauge)
                    {
                        return snmpGauge.ToUInt32();
                    }
                    else if (result[0].Data is Counter32 counter32)
                    {
                        return counter32.ToUInt32();
                    }
                    else if (result[0].Data is Counter64 counter64)
                    {
                        return counter64.ToUInt64();
                    }
                    else if (result[0].Data is Integer32 integerNumber)
                    {
                        return integerNumber.ToInt32();
                    }
                }
            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return 0;
        }
        public async Task<double> GetDoubleAsync(string ip, string communityString, string oid, int port = 161)
        {
            return await GetDoubleAsync(new IPEndPoint(IPAddress.Parse(ip), port), new OctetString(communityString), new ObjectIdentifier(oid));
        }

        public async Task<long> GetLongAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid)
        {

            if (!await PingAsync(ip.Address.ToString()))
                return 0;

            var vcl = new[] { new Variable(oid) };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            try
            {
                    

                    var result = await Messenger.GetAsync(VersionCode.V2,
                    ip, communityString,
                    vcl).WaitAsync(cts.Token);

                if (result != null && result?.Count > 0)
                {
                    if (result[0].Data is Gauge32 snmpGauge)
                    {
                        return (int)snmpGauge.ToUInt32();
                    }
                    else if (result[0].Data is Counter32 counter32)
                    {
                        return counter32.ToUInt32();
                    }

                }
            }
            catch (OperationCanceledException) { }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return 0;


        }
        public async Task<long> GetLongAsync(string ip, string communityString, string oid, int port = 161)
        {
            return await GetLongAsync(new IPEndPoint(IPAddress.Parse(ip), port), new OctetString(communityString), new ObjectIdentifier(oid));
        }

        public async Task<int> GetIntAsyn(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid)
        {
            if (!await PingAsync(ip.Address.ToString()))
                return 0;
            var vcl = new[] { new Variable(oid) };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            try
            {

                var result = await Messenger.GetAsync(VersionCode.V2,
                    ip, communityString,
                    vcl).WaitAsync(cts.Token);

                if (result != null && result.Count > 0)
                {
                    if (result[0].Data is Gauge32 snmpGauge)
                    {
                        return (int)snmpGauge.ToUInt32();
                    }
                    else if (result[0].Data is Integer32 intValue)
                    {
                        return intValue.ToInt32();
                    }
                    
                }
            }
            catch(System.Net.Sockets.SocketException socketEx)
            {
                Logger.LogCriticalException(socketEx, socketEx.Message);
            }
            catch(Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return 0;
        }
        public async Task<List<(string oid, double result)>> GetGroupDoubleAsync(string ip, string communityString, List<string> OIDs, int port = 161)
        {

            List<(string oid, double result)> results = new();
            foreach (var o in OIDs)
            {
                var result = await GetDoubleAsync(ip, communityString, o);
                results.Add((o, result));
            }
            return results;
        }
        public async Task<int> GetIntAsync(string ip, string communityString, string oid, int port = 161)
        {
            return await GetIntAsyn(new IPEndPoint(IPAddress.Parse(ip), 161), new OctetString(communityString), new ObjectIdentifier(oid));
        }
        public async Task<string> GetStringAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid)
        {
            if (!await PingAsync(ip.Address.ToString()))
                return string.Empty;
            var vcl = new [] { new Variable(oid) };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            try
            {
                var result = await Messenger.GetAsync(VersionCode.V2, ip, communityString, vcl).WaitAsync(cts.Token);

                if (result != null && result.Count > 0)
                {
                    if (result[0].Data is OctetString octet)
                    {
                        return octet.ToString();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                
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
            int timeoutMilliseconds = 2000; 
            try
            {
                using var cts = new CancellationTokenSource(timeoutMilliseconds);
                await Messenger.WalkAsync(VersionCode.V2,
                ip,
                communityString, oid,
                internalList,
                WalkMode.WithinSubtree, cts.Token).WaitAsync(cts.Token);
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

        public async Task<List<SnmpVariable>> GetBulkAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid, int maxRepetation = 10)
        {
            var internalList = new List<Variable>();
            var _snmpVariables = new List<SnmpVariable>();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            try
            {
                  await Messenger.BulkWalkAsync(VersionCode.V2,
                    ip,
                    communityString,
                    OctetString.Empty,
                    oid,
                    internalList,
                    maxRepetation,
                    WalkMode.WithinSubtree,
                    null,
                    null).WaitAsync(cts.Token);

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
        public async Task<List<SnmpVariable>> GetBulkAsync(string ip, string communityString, string oid, int maxRepetation = 10)
        {
            return await GetBulkAsync(new IPEndPoint(IPAddress.Parse(ip), 161), new OctetString(communityString), new ObjectIdentifier(oid));
        }
    }
}
