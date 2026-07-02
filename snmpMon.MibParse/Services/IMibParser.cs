using snmpMon.MibParse.Models;

namespace snmpMon.MibJson.Services
{
    public interface IMibParser
    {
        MibNode GetNode(string nodeName);
        void ParseMibJson();
    }
}