using Microsoft.Extensions.DependencyInjection;
using snmpMon.Resources.Constrols.LabelControls;
using snmpMon.ViewModel;
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

namespace snmpMon.Resources.Constrols.BoxesControls
{
    /// <summary>
    /// Interaction logic for AlertingBox.xaml
    /// </summary>
    public partial class AlertingBox : UserControl
    {
        public AlertingBox()
        {
            InitializeComponent();

        }



        public string RouterIP
        {
            get { return (string)GetValue(RouterIPProperty); }
            set { SetValue(RouterIPProperty, value); }
        }

        public static readonly DependencyProperty RouterIPProperty =
            DependencyProperty.Register("RouterIP", typeof(string), typeof(AlertingBox), new PropertyMetadata(string.Empty, OnRouterIpChanged));



        public Visibility IsDeviceNameVisiable
        {
            get { return (Visibility)GetValue(IsDeviceNameVisiableProperty); }
            set { SetValue(IsDeviceNameVisiableProperty, value); }
        }

        public static readonly DependencyProperty IsDeviceNameVisiableProperty =
            DependencyProperty.Register("IsDeviceNameVisiable", typeof(Visibility), typeof(AlertingBox), new PropertyMetadata(Visibility.Hidden));



        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register("DeviceName", typeof(string), typeof(AlertingBox), new PropertyMetadata(string.Empty));



        private static void OnRouterIpChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AlertingBox;

            if (control?.DataContext is AlertViewModel alertVM)
            {
                alertVM.RouterIP = e.NewValue as string;
            }
        }


    }
}
