using snmpMon.MibJson.Models;
using System.Threading.Tasks;

namespace snmpMon.Services
{
    public interface IManageTemplateServices
    {
        Task<bool> ConfirmNode(JsonNode node, string TemplateName);
    }
}