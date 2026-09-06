using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.MibJson.Models
{
    public class JsonNode
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("resourcesOid")]
        public ResourcesOIDs? Resources { get; set; }
        [JsonProperty("vendor")]
        public string? Vendor { get; set; }
        [JsonProperty("unit")]
        public string? Unit { get; set; }

    }

    public class ResourcesOIDs
    {
        [JsonProperty("memoryUsed")]
        public string? MemoryUsed {  get; set; }
        [JsonProperty("memoryFree")]
        public string? MemoryFree {  get; set; }
        [JsonProperty("cpuUsage")]
        public string? CpuUsage {  get; set; }
        [JsonProperty("deviceTemperature")]
        public string? DeviceTemperature {  get; set; }
    }
}
