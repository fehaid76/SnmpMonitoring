
using Lextm.SharpSnmpLib;

namespace snmpMon.Engine.Services.SnmpTraslatorServices
{
    public interface ISnmpSeverityAnalyzer
    {
        Task StartSeverityLevelsMapping();
        void FetchSnmpTrapVariablesAndPDU(IList<Variable> snmpTraps, string trapPDU);
        int GetSeverityLevel();
    }
}