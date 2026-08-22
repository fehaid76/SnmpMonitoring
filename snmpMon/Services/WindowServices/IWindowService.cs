using snmpMon.Data.Model;
using snmpMon.Models;
using snmpMon.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace snmpMon.Services.WindowServices
{
    public interface IWindowService
    {
        event Action<Device> OnDeviceDetails;

        void ShowAddDeviceWindow();
        void ShowDeviceMonitoringWin(DeviceInfo device);
        bool ShowConfirmDialog(string message, string title, MessageBoxIcon icon);
        void ShowInterfaceOverview(string ipAddress, string CommunityString, string ifIndex, string ifDesc);


    }
}
