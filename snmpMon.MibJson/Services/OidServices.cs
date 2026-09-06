using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.MibJson.Services
{
    public class OidServices
    {
        List<string> nodes = new();

        
        public Task<MibNode> GetMemoryNode(string nodeName)
        {
            throw new NotImplementedException();
        }

        public Task<MibNode> GetNode(string nodeName)
        {
            throw new NotImplementedException();
        }

        public Task<MibNode> GetProccessMibNode(string nodeName)
        {
            throw new NotImplementedException();
        }

        
    }
}
