using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Data.Model
{
    public class Device
    {
        public int ID {  get; set; }
        public string Name { get; set; }
        public string Vendor {  get; set; }
        public string IPAddress {  get; set; }
        public string CommunityString {  get; set; }


        public Device(int iD, string name, string vendor, string iPAddress, string communityString)
        {
            ID = iD;
            Name = name;
            Vendor = vendor;
            IPAddress = iPAddress;
            CommunityString = communityString;
        }

        public Device(string iPAddress, string communityString)
        {

            IPAddress = iPAddress;
            CommunityString = communityString;
        }

        public Device() { }




    }
}
