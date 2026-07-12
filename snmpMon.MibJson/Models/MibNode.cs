using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace snmpMon.MibParse.Models
{
    public class MibNode
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("oid")]
        public string? Oid { get; set; }

        [JsonProperty("nodetype")]
        public string? NodeType { get; set; } 

        [JsonProperty("class")]
        public string? Class { get; set; }

        [JsonProperty("syntax")]
        public SyntaxInfo? Syntax { get; set; } 

        [JsonProperty("description")] 
        public string? Desc { get; set; }

        
        public string? ActualType => Syntax?.Type;
    }

    public class SyntaxInfo
    {
        [JsonProperty("type")]
        public string? Type { get; set; } 

        [JsonProperty("class")]
        public string? Class { get; set; }
    }
}
