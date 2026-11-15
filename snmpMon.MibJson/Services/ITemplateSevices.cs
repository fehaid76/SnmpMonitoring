using snmpMon.MibJson.Models;
using snmpMon.MibParse.Models;

namespace snmpMon.MibJson.Services
{
    public interface ITemplateSevices
    {
        string ConvertToJson(JsonNode node);
        List<JsonNode> GetGroupOfJsonNodes(List<string> paths);
        void RemoveJsonFile(string path);
        void ModifyExistsFile(JsonNode node, string path);
    }
}