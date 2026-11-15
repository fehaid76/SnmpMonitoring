using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snmpMon.Services
{
    public interface IManageTemplateServices
    {
        event EventHandler<JsonNodeEventArgs> OnJsonNodeAdd;
        Task<bool> ConfirmNode(JsonNode node, string TemplateName);
        Task<List<JsonNode>> GetAllTemplates();
        Task<bool> DeleteTemplate(string categoryName, string path);
        void ModifyExistsTemplate(JsonNode node, string path);
    }
}