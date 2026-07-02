using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Core.Utilities
{
    public static class UtilityCalculating
    {
        public static int GetHostsCountInSubnet(string subnetmask)
        {
            if (subnetmask == "255.255.255.255")
                return 1;
            
            IPAddress _subnetmask = IPAddress.Parse(subnetmask);
            byte[] _getBytes = _subnetmask.GetAddressBytes();
            int cird = 0;
            foreach (byte b in _getBytes)
            {
                cird += Convert.ToString(b, 2).Replace("0", "").Length;

            }

            int hostBits = 32 - cird;
            return Convert.ToInt32(Math.Pow(2, hostBits) - 2);
        }
        public static string CalculateIpAddresses(string subnetBase, int index)
        {
            IPAddress baseIP = IPAddress.Parse(subnetBase);
            byte[] ipBytes = baseIP.GetAddressBytes();

            ipBytes[2] |= (byte)((index >> 8) & 0xFF);
            ipBytes[3] |= (byte)(index & 0xFF);

            return new IPAddress(ipBytes).ToString();
        }
        public static string GetBaseIP(string ipAddress, string subnetmask)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPAddress mask = IPAddress.Parse(subnetmask);

            byte[] ipBytes = ip.GetAddressBytes();
            byte[] subnetBytes = mask.GetAddressBytes();
            byte[] result = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                result[i] = (byte)(ipBytes[i] & subnetBytes[i]); 

            }
            return new IPAddress(result).ToString();
         }

    }
}
