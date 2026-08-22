using snmpMon.Helper.EventHandle;
using System;
using System.Threading.Tasks;

namespace snmpMon.Services.SensorServices
{
    public interface IInterfacesSensors
    {
        event EventHandler<TrafficEventArgs> TrafficDataReady;
        Task StartFetchInterfaceDetails(string ipAddress, string communityString, string ifIndex);
    }
}