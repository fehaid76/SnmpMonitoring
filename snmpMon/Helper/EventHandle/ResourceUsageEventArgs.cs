using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Helper.EventHandle
{
    public class ResourceUsageEventArgs : EventArgs
    {

        public double MemoryUsage {  get; set; }
        public double CPUutilization { get; set; }

        public ResourceUsageEventArgs(double MemoryUsage, double CPUutilization)
        {
            this.MemoryUsage = MemoryUsage;
            this.CPUutilization = CPUutilization;
        }

        
    }
}
