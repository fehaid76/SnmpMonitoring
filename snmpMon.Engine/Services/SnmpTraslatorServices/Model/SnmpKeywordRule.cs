using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Services.SnmpTraslatorServices.Model
{
    public class SnmpLogConfig
    {
        public List<SnmpKeywordRule>? NetworkEvents {  get; set; }
    }

    public class SnmpKeywordRule
    {
        [JsonProperty("Oid")]
        public string? Oid {  get; set; }
        [JsonProperty("Name")]
        public string? Name { get; set; }
        [JsonProperty("Severity")]
        public int? Severity { get; set; }
        [JsonProperty("Description")]
        public string? Description { get; set; }
    }
}
