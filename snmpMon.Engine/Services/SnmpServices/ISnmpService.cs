using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using snmpMon.Engine.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpServices
{
    public interface ISnmpService
    {
        Task<int> GetIntAsyn(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid);
        Task<int> GetIntAsync(string ip, string communityString, string oid, int port = 161);
        Task<long> GetLongAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid);
        Task<long> GetLongAsync(string ip, string communityString, string oid, int port = 161);
        Task<string> GetStringAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid);
        Task<string> GetStringAsync(string ip, string communityString, string oid, int port = 161);
        Task<TimeSpan> GetUpTime(string ip, string communityString, string oid);
        Task<List<Variable>> GetWalkAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid);
        Task<List<Variable>> GetWalkAsync(string ip, string communityString, string oid, int port = 161);
        Task<List<SnmpVariable>> GetBulkAsync(string ip, string communityString, string oid, int maxRepetation = 10);
        Task<List<SnmpVariable>> GetBulkAsync(IPEndPoint ip, OctetString communityString, ObjectIdentifier oid, int maxRepetation = 10);
    }
}
