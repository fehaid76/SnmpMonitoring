using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib.Mib;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using snmpMon.MibParse.Models;
using System.Net.Http;
using System.Text.Json.Nodes;
using snmpMon.MibJson.Models;
using JsonNode = snmpMon.MibJson.Models.JsonNode;

namespace snmpMon.MibJson.Services
{
    public class MibParser : IMibParser
    {
        Dictionary<string, MibNode> _genericMibTree;
        Dictionary<string, JsonNode> _jsonTemplatesNodes; 
        private readonly HttpClient _httpClient;
        private TaskCompletionSource<bool> _initializationTcs = new TaskCompletionSource<bool>();

        public MibParser()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# HttpClient/snmpMonAPP");
        }

        
        

        private async Task PrepareOidsTemplates(List<string> paths)
        {
            var templatesString = await ReadJsonFiles(paths);
            var Items = ProcessAllJsonFiles(templatesString);
            ConvertJsonNodesToDictionary(Items);

        }

        private async Task<List<string>> ReadJsonFiles(List<string> Paths)
        {
            List<string> templatesString = new();
            string tempString = string.Empty;
            foreach (var Path in Paths)
            {
                tempString = await File.ReadAllTextAsync(Path);
                templatesString.Add(tempString);
            }

            return templatesString;
        }

        private List<JsonNode> ProcessAllJsonFiles(List<string> templates)
        {
            List<JsonNode> result = new();
            foreach (var path in templates)
            {
                var jsonDeserialized = JsonConvert.DeserializeObject<JsonNode>(path);
                result.Add(jsonDeserialized);
            }
            return result;
        }

        private void ConvertJsonNodesToDictionary(List<JsonNode> jsonNodes)
        {
            _jsonTemplatesNodes = jsonNodes.ToDictionary(item => item.Name ?? string.Empty, item => item);
        }

        public JsonNode GetDeviceNode(string key)
        {
            if(_jsonTemplatesNodes != null && _jsonTemplatesNodes.TryGetValue(key, out var node))
            {
                return node;
            }

            return new JsonNode();
        }
        
        public async Task ParseMibJson(List<string> paths)
        {
            await PrepareOidsTemplates(paths);  
            if (_initializationTcs.Task.IsCompleted) return;
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                string genericOidUrl = "https://raw.githubusercontent.com/fehaid76/mib_json/refs/heads/main/vendor-generic.json";

                string genericOidjsonString = await _httpClient.GetStringAsync(genericOidUrl, cts.Token);
                if (!string.IsNullOrEmpty(genericOidjsonString))
                {
                    var genericOidResult = JsonConvert.DeserializeObject<Dictionary<string, MibNode>>(genericOidjsonString);
                    _genericMibTree = genericOidResult ?? new Dictionary<string, MibNode>();
                    
                }

                _initializationTcs.TrySetResult(true);

            }
            catch (Exception ex)
            {
                _genericMibTree = new Dictionary<string, MibNode>();
                _initializationTcs.TrySetResult(false);
                Debug.WriteLine(ex.Message);

            }
        }

        public async Task<MibNode> GetNode(string nodeName)
        {
            await _initializationTcs.Task;

            if (_genericMibTree != null && _genericMibTree.TryGetValue(nodeName, out var node))
            {
                return node;
            }
            return new MibNode();
        }
    }
}
