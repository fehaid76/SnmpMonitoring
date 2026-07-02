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

namespace snmpMon.Resources.Constrols.LabelControls
{
    /// <summary>
    /// Interaction logic for InterfacesView.xaml
    /// </summary>
    public partial class InterfacesView : UserControl
    {
        public InterfacesView()
        {
            InitializeComponent();
        }





        public string PortDescr
        {
            get { return (string)GetValue(PortDescrProperty); }
            set { SetValue(PortDescrProperty, value); }
        }

        public static readonly DependencyProperty PortDescrProperty =
            DependencyProperty.Register("PortDescr", typeof(string), typeof(InterfacesView), new PropertyMetadata(string.Empty));



        public string PortType
        {
            get { return (string)GetValue(PortTypeProperty); }
            set { SetValue(PortTypeProperty, value); }
        }

        public static readonly DependencyProperty PortTypeProperty =
            DependencyProperty.Register("PortType", typeof(string), typeof(InterfacesView), new PropertyMetadata(string.Empty));



        public string PortSpeed
        {
            get { return (string)GetValue(PortSpeedProperty); }
            set { SetValue(PortSpeedProperty, value); }
        }

        public static readonly DependencyProperty PortSpeedProperty =
            DependencyProperty.Register("PortSpeed", typeof(string), typeof(InterfacesView), new PropertyMetadata(string.Empty));



        public string AdminStatus
        {
            get { return (string)GetValue(AdminStatusProperty); }
            set { SetValue(AdminStatusProperty, value); }
        }

        public static readonly DependencyProperty AdminStatusProperty =
            DependencyProperty.Register("AdminStatus", typeof(string), typeof(InterfacesView), new PropertyMetadata(string.Empty));



        public string CurrentStatus
        {
            get { return (string)GetValue(CurrentStatusProperty); }
            set { SetValue(CurrentStatusProperty, value); }
        }

        public static readonly DependencyProperty CurrentStatusProperty =
            DependencyProperty.Register("CurrentStatus", typeof(string), typeof(InterfacesView), new PropertyMetadata(string.Empty));




        public ICommand PortCommand
        {
            get { return (ICommand)GetValue(PortCommandProperty); }
            set { SetValue(PortCommandProperty, value); }
        }

        public static readonly DependencyProperty PortCommandProperty =
            DependencyProperty.Register("PortCommand", typeof(ICommand), typeof(InterfacesView), new PropertyMetadata(default));



        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InterfacesView), new PropertyMetadata(default));









    }
}
