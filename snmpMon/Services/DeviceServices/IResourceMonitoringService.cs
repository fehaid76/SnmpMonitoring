using Lextm.SharpSnmpLib;
using snmpMon.Helper.EventHandle;
using System;
using System.Net;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public interface IResourceMonitoringService
    {
        event EventHandler<ResourceUsageEventArgs> OnResourceDeviceUsage;

        (string MemoryUsage, string cpuTotal1Min) FetchUtilizationNodes(string ipAddress, string communityString, string deviceCategory);
        Task GetResourceUsage(IPEndPoint ip, OctetString communityString, string MemoryUsageOid, string cpuUtilizationOid);
    }
}