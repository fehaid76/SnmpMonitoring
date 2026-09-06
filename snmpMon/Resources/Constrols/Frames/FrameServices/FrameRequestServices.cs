using snmpMon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace snmpMon.Resources.Constrols.Frames.FrameServices
{
    public class FrameRequestServices : IFrameRequestServices
    {
        private readonly Devices devices = new();
        private readonly Dashboard dashboard = new();
        private readonly SnmpAlerts snmpAlerts = new();
        private readonly DeviceTemplates deviceTemplates = new();


        public UserControl GetDevicesFrame()
        {

            return devices;
        }

        public UserControl GetOverviewFrame()
        {
            return dashboard;
        }

        public UserControl GetAlertsFrame()
        {
            return snmpAlerts;
        }

        public UserControl GetDeviceTemplatesFrame()
        {
            return deviceTemplates;
        }





    }
}
