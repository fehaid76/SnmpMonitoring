using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Models.WindowAgregation
{
    public class WindowAggregation<T, U>
    {
        
        public T MainGenericViewModel {  get; set; }
        public U SubGenericViewModel {  get; set; }
    }
}
