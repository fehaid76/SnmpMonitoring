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
        public int CPUutilization { get; set; }

        public ResourceUsageEventArgs(double MemoryUsage, int CPUutilization)
        {
            this.MemoryUsage = MemoryUsage;
            this.CPUutilization = CPUutilization;
        }


    }
}
