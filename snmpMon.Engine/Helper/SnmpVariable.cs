using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Helper
{
    public class SnmpVariable
    {
        public string Oid {  get; set; }
        public object Data {  get; set; }

        public SnmpVariable(string oid, object data)
        {
            this.Oid = oid;
            this.Data = data;
        }
    }
}
