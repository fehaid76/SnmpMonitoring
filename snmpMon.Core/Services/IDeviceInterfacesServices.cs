using snmpMon.Data.Model;

namespace snmpMon.Core.Services
{
    public interface IDeviceInterfacesServices
    {
        
        Task DeviceInboundSpeedKbps(List<Device> devices, string oid);
        double CalculatTotalDeviceInboundSpeedKbps();
        string GetTopInboundDevice();
    }
}