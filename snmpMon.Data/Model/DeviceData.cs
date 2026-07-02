using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Model
{
    public class DeviceData
    {
        public int ID { get; set; }
        public string IPAddress {  get; set; }
        public string CommunityString {  get; set; }

        public DeviceData(string ipAddress, string communityString)
        {
            this.IPAddress = ipAddress;
            this.CommunityString = communityString;
        }

        public DeviceData()
        {
            
        }
    }
}
