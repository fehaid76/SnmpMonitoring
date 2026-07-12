
using snmpMon.MibParse.Models;

namespace snmpMon.Core.Services
{
    public interface IDeviceVerficationServices
    {
        event Action<bool> OnPingSuccess;
        event Action<bool> OnSnmpProtocolSuccess;
        event Action<string> OnDeviceVendorRecieved;
        event Action<bool> OnSnmpEnabled;

        Task StartVerificationDevice(string ipAddress, string communityString, MibNode node);
    }
}