using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.Helper.EventHandle;
using snmpMon.MibJson.Models;
using snmpMon.MibJson.Services;
using snmpMon.Services;
using snmpMon.Services.WindowServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.ViewModels
{
    public partial  class TemplatesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<JsonNode> _templates = new();

        [ObservableProperty]
        private JsonNode _selectedNode = new();

        public ICommand ModifyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private readonly IManageTemplateServices _manageTemplateServices;
        private readonly IWindowService _windowService;
        public TemplatesViewModel(IManageTemplateServices manageTemplateServices, IWindowService windowService)
        {
           _manageTemplateServices = manageTemplateServices;
            _windowService = windowService;
            _manageTemplateServices.OnJsonNodeAdd += EventNodeHandler;
            _= LoadAllTemplates();

            ModifyCommand = new RelayCommand(() =>
            {
                GetModifyTemplateWindow(); 
            });

            DeleteCommand = new RelayCommand(async ()=>
            {
                await DeleteTemplate();
            });
        }

        private void GetModifyTemplateWindow()
        {
            _windowService.ModifyTemplateWindow(SelectedNode);
        }

        private void EventNodeHandler(object sender, JsonNodeEventArgs e)
        {
            if(e.Node != null)
                Templates.Add(e.Node);
        }

        private async Task LoadAllTemplates()
        {
            var templates = await _manageTemplateServices.GetAllTemplates();
            for (int i = 0; i < templates.Count; i++)
            {
                templates[i].Index = i + 1;
                Templates.Add(templates[i]);
            }
        }

        private bool DeleteConfirm()
        {
            var result = _windowService.ShowConfirmDialog("Are you sure you want to delete it?", "Confirm dialog", MessageBoxIcon.Question);
            return result;
        }

        private async Task DeleteTemplate()
        {
            if (!DeleteConfirm())
                return;

            var isDeleted = await _manageTemplateServices.DeleteTemplate(SelectedNode.Name, SelectedNode.TemplatePath);
            if(isDeleted)
            {
                var itemToRemove = Templates.FirstOrDefault(t => t.Name == SelectedNode.Name);
                if(itemToRemove != null)
                {
                    Templates.Remove(itemToRemove);
                }
            }
        }
    }
}
