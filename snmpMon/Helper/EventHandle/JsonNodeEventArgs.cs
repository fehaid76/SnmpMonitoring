using snmpMon.Enums;
using snmpMon.MibJson.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Helper.EventHandle
{
    public  class JsonNodeEventArgs : EventArgs
    {
        public JsonNode Node { get; set; }

        public JsonNodeEventArgs(JsonNode node)
        {
            Node = node;
        }
    }
}
