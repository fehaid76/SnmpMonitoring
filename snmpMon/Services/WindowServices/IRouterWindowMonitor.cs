using snmpMon.Models;

namespace snmpMon.Services.WindowServices
{
    public interface IRouterWindowMonitor
    {
        void OpenMonitoringWindow(DeviceInfo device);
    }
}