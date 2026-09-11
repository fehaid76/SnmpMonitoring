using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.MibJson.Models;
using snmpMon.MibParse.Models;
using snmpMon.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace snmpMon.ViewModels
{
    public class NewTemplateViewModel : ObservableObject
    {
        
        private JsonNode _jsonNode = new();
        public JsonNode JsonNode
        {
            get => _jsonNode;
            set
            {
                SetProperty(ref _jsonNode, value);
            }
        }
        private ResourcesOIDs _resourcesOIDs = new();
        public ResourcesOIDs ResourcesOIDs
        {
            get => _resourcesOIDs;
            set
            {
                SetProperty(ref _resourcesOIDs, value);
            }
        }

        private MemoryConfig _memoryConfig = new();

        public MemoryConfig MemoryConfig
        {
            get => _memoryConfig;
            set
            {
                _memoryConfig = value;
                SetProperty(ref _memoryConfig, value);
            }
        }

        public ICommand ConfirmCommand {  get; set; }
        private readonly IManageTemplateServices _manageTemplateServices;
        public NewTemplateViewModel(IManageTemplateServices manageTemplateServices)
        {
            _manageTemplateServices = manageTemplateServices;

            ConfirmCommand = new RelayCommand(async () =>
            {
                if (await ConfirmNode())
                    ClearFields();
            });
        }

        private async Task<bool> ConfirmNode()
        {
            if(JsonNode != null)
            {
                _jsonNode.Resources = ResourcesOIDs;
                ResourcesOIDs.MemoryConfig = MemoryConfig;
                return await _manageTemplateServices.ConfirmNode(JsonNode, JsonNode.Name);
            }
            return false;
        } 

        private void ClearFields()
        {
            if (JsonNode != null)
            {
                JsonNode = new();
                ResourcesOIDs = new();
                MemoryConfig = new();
            }
                
        }


    }
}
