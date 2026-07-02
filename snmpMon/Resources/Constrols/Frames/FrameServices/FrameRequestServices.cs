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
        private readonly Devices devices = new Devices();
        public UserControl GetDevicesFrame()
        {
            return devices;
        }
    }
}
