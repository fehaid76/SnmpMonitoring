using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using snmpMon.MibJson.Models;
using snmpMon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.ViewModels
{
    public partial class ModifyTemplateViewModel : ObservableObject
    {
        [ObservableProperty]
        private JsonNode _node = new();
        [ObservableProperty]
        private MemoryConfig _memoryConfig = new();

        public ICommand ModifyCommand {  get; set; }
        private readonly IManageTemplateServices _manageTemplateServices;
        public ModifyTemplateViewModel(IManageTemplateServices manageTemplateServices)
        {
            _manageTemplateServices = manageTemplateServices;

            ModifyCommand = new RelayCommand<Window>(w =>
            {
                ModifyTemplate();
                ClearObject(w);
            });
        }

        public void FetchNodeObject(JsonNode node)
        {
            Node = node;
        }

        private void ClearObject(Window w)
        {
            Node = new();
            w.Close();
        }
        private void ModifyTemplate()
        {
            var path = Node.TemplatePath;
            _manageTemplateServices.ModifyExistsTemplate(Node, path);
        }
    }
}
