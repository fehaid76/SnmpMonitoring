using snmpMon.Helper;
using snmpMon.Helper.Model;
using snmpMon.Models;
using System;
using System.Threading.Tasks;

namespace snmpMon.Services.Agent
{
    public interface IMonitoringAgentServices
    {

        Task<ActionDisposableAggregation> StartMonitoring(string ipAddress, string communityString, DeviceInfo deviceInfo);
    }
}