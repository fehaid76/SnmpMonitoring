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

namespace snmpMon.MibJson.Services
{
    public class MibParser : IMibParser
    {
        Dictionary<string, MibNode> _mibTree;

        
        public void ParseMibJson()
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string _filePath = Path.Combine(baseDirectory, "MBFiles", "CoreMibs", "vendor-generic.json");
                string jsonString = File.ReadAllText(_filePath);

                _mibTree = JsonConvert.DeserializeObject<Dictionary<string, MibNode>>(jsonString);
            }
            catch (Exception ex)
            {
                _mibTree = new Dictionary<string, MibNode>();

            }
        }

        public MibNode GetNode(string nodeName)
        {
            if (_mibTree != null && _mibTree.TryGetValue(nodeName, out var node))
            {
                return node;
            }
            return null;
        }
    }
}
