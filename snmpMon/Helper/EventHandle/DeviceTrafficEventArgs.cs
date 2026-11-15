using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Helper.EventHandle
{
    public class DeviceTrafficEventArgs : EventArgs
    {
        public double InboundTraffic {  get; set; }
        public double OutboundTraffic { get; set; }
        public string TopConsumerDevice {  get; set; }

        public DeviceTrafficEventArgs(double inboundTraffic, double outboundTraffic, string topConsumerDevice)
        {
            InboundTraffic = inboundTraffic;
            OutboundTraffic = outboundTraffic;
            TopConsumerDevice = topConsumerDevice;
        }

        public DeviceTrafficEventArgs(double inboundTraffic, double outboundTraffic)
        {
            InboundTraffic = inboundTraffic;
            OutboundTraffic = outboundTraffic;
        }

    }
}
