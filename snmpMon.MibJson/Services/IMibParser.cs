using snmpMon.MibParse.Models;
using System.Threading.Tasks;

namespace snmpMon.MibJson.Services
{
    public interface IMibParser
    {
        MibNode GetNode(string nodeName);
        Task ParseMibJson();
    }
}