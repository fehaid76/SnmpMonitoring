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

namespace snmpMon.MibJson.Services
{
    public class MibParser : IMibParser
    {
        Dictionary<string, MibNode> _mibTree;
        private readonly HttpClient _httpClient;

        public MibParser()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# HttpClient/snmpMonAPP");
        }

        public async Task ParseMibJson()
        {
            try
            {

                string url = "https://raw.githubusercontent.com/fehaid76/mib_json/refs/heads/main/vendor-generic.json";
                string jsonString = await _httpClient.GetStringAsync(url);
                if (!string.IsNullOrEmpty(jsonString))
                {
                    _mibTree = JsonConvert.DeserializeObject<Dictionary<string, MibNode>>(jsonString);

                }



            }
            catch (HttpRequestException ex)
            {
                _mibTree = new Dictionary<string, MibNode>();
                Debug.WriteLine(ex.Message);
            }
            catch (JsonException ex)
            {
                _mibTree = new Dictionary<string, MibNode>();
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                _mibTree = new Dictionary<string, MibNode>();
                Debug.WriteLine(ex.Message);

            }
        }

        public MibNode GetNode(string nodeName)
        {
            if (_mibTree != null && _mibTree.TryGetValue(nodeName, out var node))
            {
                return node;
            }
            return new MibNode();
        }
    }
}
