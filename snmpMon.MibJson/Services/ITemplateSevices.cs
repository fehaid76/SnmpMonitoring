using snmpMon.MibJson.Models;
using snmpMon.MibParse.Models;

namespace snmpMon.MibJson.Services
{
    public interface ITemplateSevices
    {
        string ConvertToJson(JsonNode node);
    }
}