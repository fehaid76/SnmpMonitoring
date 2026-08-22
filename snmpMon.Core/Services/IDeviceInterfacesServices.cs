using snmpMon.Data.Model;

namespace snmpMon.Core.Services
{
    public interface IDeviceInterfacesServices
    {
        
        Task DeviceTrafficSpeedKbps(List<Device> devices, string oid);
        double CalculatTotalDeviceTrafficSpeedKbps();
        string GetTopInboundDevice();
    }
}