using snmpMon.MibParse.Models;
using System.Threading.Tasks;

namespace snmpMon.MibJson.Services
{
    public interface IMibParser
    {
        Task<MibNode> GetNode(string nodeName);
        Task ParseMibJson();
    }
}