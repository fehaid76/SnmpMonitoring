using snmpMon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Helper.EventHandle
{
    public class DeviceStatusEventArgs : EventArgs
    {
        public DeviceStatus DeviceStatus { get; set; }
        public string IPAddress {  get; set; }
        public string CommunityString {  get; set; }
        public DeviceStatusEventArgs(string IPAddress, string CommunityString , DeviceStatus DeviceStatus)
        {
            this.IPAddress = IPAddress;
            this.DeviceStatus = DeviceStatus;
            this.CommunityString = CommunityString;
        }

        public bool CheckForDeviceStatus()
        {
            return this.DeviceStatus == DeviceStatus.Up; 
        }
    }
}
