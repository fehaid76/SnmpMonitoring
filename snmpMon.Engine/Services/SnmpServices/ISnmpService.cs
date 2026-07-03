using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using snmpMon.Engine.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpServices
{
    public interface ISnmpService
    {
        Task<int> GetIntAsyn(string ip, string communityString, string oid);
        Task<TimeSpan> GetUpTime(string ip, string communityString, string oid);
        Task<List<Variable>> GetWalkAsync(string ip, string communityString, string oid);
        Task<List<SnmpVariable>> GetBulkAsync(string ip, string communityString, string oid, int maxRepetation = 10);
    }
}
