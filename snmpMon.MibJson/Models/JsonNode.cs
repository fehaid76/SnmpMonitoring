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
        public int Index {  get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("resourcesOid")]
        public ResourcesOIDs? Resources { get; set; }
        [JsonProperty("vendor")]
        public string? Vendor { get; set; }
        [JsonProperty("unit")]
        public string? Unit { get; set; }
        public string? TemplatePath {  get; set; }
    }

    public class ResourcesOIDs
    {

        [JsonProperty("memoryConfig")]
        public MemoryConfig? MemoryConfig { get; set; }
        [JsonProperty("cpuUsage")]
        public string? CpuUsage {  get; set; }
        [JsonProperty("deviceTemperature")]
        public string? DeviceTemperature {  get; set; }
    }

    public class MemoryConfig
    {
        [JsonProperty("memoryInputType")]
        public int MemoryInputType {  get; set; }
        [JsonProperty("memoryUsedOid")]
        public string? MemoryOid {  get; set; }
        [JsonProperty("memoryFreeOid")]
        public string? MemoryFreeOid { get;set; }
        [JsonProperty("memoryUsageOid")]
        public string? MemoryUsageOID {  get; set; }
    }
}
