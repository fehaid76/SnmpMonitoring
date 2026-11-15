using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace snmpMon.Resources.Constrols.InputControls.ConfigurationInputs
{
    /// <summary>
    /// Interaction logic for TemplateInformation.xaml
    /// </summary>
    public partial class TemplateInformation : UserControl
    {
        public TemplateInformation()
        {
            InitializeComponent();
        }




        public object CommandPar
        {
            get { return (object)GetValue(CommandParProperty); }
            set { SetValue(CommandParProperty, value); }
        }

        public static readonly DependencyProperty CommandParProperty =
            DependencyProperty.Register("CommandPar", typeof(object), typeof(TemplateInformation), new PropertyMetadata(default));



        public bool TemplateNameIsReadOnly
        {
            get { return (bool)GetValue(TemplateNameIsReadOnlyProperty); }
            set { SetValue(TemplateNameIsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty TemplateNameIsReadOnlyProperty =
            DependencyProperty.Register("TemplateNameIsReadOnly", typeof(bool), typeof(TemplateInformation), new PropertyMetadata(false));



        public int SelectedType
        {
            get { return (int)GetValue(SelectedTypeProperty); }
            set { SetValue(SelectedTypeProperty, value); }
        }

        public static readonly DependencyProperty SelectedTypeProperty =
            DependencyProperty.Register("SelectedType", typeof(int), typeof(TemplateInformation), new PropertyMetadata(default));


        public string TemplateName
        {
            get { return (string)GetValue(TemplateNameProperty); }
            set { SetValue(TemplateNameProperty, value); }
        }

        public static readonly DependencyProperty TemplateNameProperty =
            DependencyProperty.Register("TemplateName", typeof(string), typeof(TemplateInformation), new PropertyMetadata(string.Empty));



        public string Vendor
        {
            get { return (string)GetValue(VendorProperty); }
            set { SetValue(VendorProperty, value); }
        }

        public static readonly DependencyProperty VendorProperty =
            DependencyProperty.Register("Vendor", typeof(string), typeof(TemplateInformation), new PropertyMetadata(string.Empty));




        public string MemoryUsed
        {
            get { return (string)GetValue(MemoryUsedProperty); }
            set { SetValue(MemoryUsedProperty, value); }
        }

        public static readonly DependencyProperty MemoryUsedProperty =
            DependencyProperty.Register("MemoryUsed", typeof(string), typeof(TemplateInformation), new PropertyMetadata(string.Empty));



        public string MemoryFree
        {
            get { return (string)GetValue(MemoryFreeProperty); }
            set { SetValue(MemoryFreeProperty, value); }
        }

        public static readonly DependencyProperty MemoryFreeProperty =
            DependencyProperty.Register("MemoryFree", typeof(string), typeof(TemplateInformation), new PropertyMetadata(string.Empty));




        public string MemoryUsage
        {
            get { return (string)GetValue(MemoryUsageProperty); }
            set { SetValue(MemoryUsageProperty, value); }
        }

        public static readonly DependencyProperty MemoryUsageProperty =
            DependencyProperty.Register("MemoryUsage", typeof(string), typeof(TemplateInformation), new PropertyMetadata(string.Empty));




        public string CPUUsage
        {
            get { return (string)GetValue(CPUUsageProperty); }
            set { SetValue(CPUUsageProperty, value); }
        }

        public static readonly DependencyProperty CPUUsageProperty =
            DependencyProperty.Register("CPUUsage", typeof(string), typeof(TemplateInformation), new PropertyMetadata(string.Empty));



        public string DeviceTemperature
        {
            get { return (string)GetValue(DeviceTemperatureProperty); }
            set { SetValue(DeviceTemperatureProperty, value); }
        }

        public static readonly DependencyProperty DeviceTemperatureProperty =
            DependencyProperty.Register("DeviceTemperature", typeof(string), typeof(TemplateInformation), new PropertyMetadata(string.Empty));



        public ICommand ConfirmCommand
        {
            get { return (ICommand)GetValue(ConfirmCommandProperty); }
            set { SetValue(ConfirmCommandProperty, value); }
        }

        public static readonly DependencyProperty ConfirmCommandProperty =
            DependencyProperty.Register("ConfirmCommand", typeof(ICommand), typeof(TemplateInformation), new PropertyMetadata(default));



    }
}
