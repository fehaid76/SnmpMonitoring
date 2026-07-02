using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using snmpMon.Engine.Helper;
using System.Threading;
using Lextm.SharpSnmpLib.Security;

namespace snmpMon.Engine.Services.SnmpServices
{
    public class SnmpService : ISnmpService
    {
        public async Task<int> GetIntAsyn(string ip, string communityString, string oid)
        {
            int snmpValue = 0; 

            var _oid = new ObjectIdentifier(oid);
            List<Variable> vcl = new List<Variable>() { new Variable(_oid) };

            try
            {
                var result = await Messenger.GetAsync(VersionCode.V2,
                    new IPEndPoint(IPAddress.Parse(ip), 161), new OctetString(communityString),
                    vcl);

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
        public async Task<List<Variable>> GetWalkAsync(string ip, string communityString, string oid)
        {
            var internalList = new List<Variable>();

            try
            {

                await Messenger.WalkAsync(VersionCode.V2,
                new IPEndPoint(IPAddress.Parse(ip), 161),
                new OctetString(communityString), new ObjectIdentifier(oid),
                internalList, WalkMode.WithinSubtree);

                return internalList;
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
                Logger.LogCriticalException(ex, ex.Message);
            }

            return new List<Variable>();
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
