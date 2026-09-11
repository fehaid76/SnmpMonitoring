using snmpMon.Enums;
using snmpMon.MibJson.Models;
using snmpMon.MibJson.Services;
using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Services
{
    public class ManageTemplateServices : IManageTemplateServices
    {
        private JsonNode jsonNodes = new();
        private readonly ITemplateSevices _templateSevices;
        private readonly IDevicesServices _devicesServices;
        public ManageTemplateServices(ITemplateSevices templateSevices, IDevicesServices devicesServices)
        {
            _templateSevices = templateSevices;
            _devicesServices = devicesServices;
        }

        public async Task<bool> ConfirmNode(JsonNode node, string TemplateName)
        {
            jsonNodes = GetResourcesNode(node);
            var serializedProcess = await SerializeNodes(TemplateName);
            return  serializedProcess;
        }
        private JsonNode GetResourcesNode(JsonNode node)
        {
            JsonNode mibNode = new JsonNode()
            {
                Name = node.Name,
                Resources = GetPreparedResourcesObject(node),
                Vendor = node.Vendor,
                Unit = node.Unit

            };

            return mibNode;
        }

        private ResourcesOIDs GetPreparedResourcesObject(JsonNode node)
        {
            ResourcesOIDs resources = new();
            resources.MemoryConfig = new();
            if(node != null)
            {
                resources.MemoryConfig.MemoryUsageOID = node.Resources.MemoryConfig.MemoryUsageOID;
                resources.CpuUsage = node.Resources.CpuUsage;
                resources.DeviceTemperature = node.Resources.DeviceTemperature;
            }

            return resources;

        }

        private async Task<bool> SerializeNodes(string TemplateName)
        {
            var jsonPath = _templateSevices.ConvertToJson(jsonNodes);
            return await LogJsonPathAsync(TemplateName, jsonPath);
        }

        private async Task<bool> LogJsonPathAsync(string TemplateName, string Path)
        {
            return await _devicesServices.AddCategoryAsync(TemplateName, Path);
        }
    }
}
