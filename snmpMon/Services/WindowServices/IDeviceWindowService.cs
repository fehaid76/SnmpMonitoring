using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace snmpMon.Services.WindowServices
{
    public interface IDeviceWindowService
    {
        event Action<DeviceAdd> OnDeviceDetails;

        void ShowAddDeviceWindow();
        void ShowDeviceMonitoringWin(DeviceInfo device);
        
    }
}
