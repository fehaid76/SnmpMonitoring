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
    /// Interaction logic for UptimeDownstreamMessage.xaml
    /// </summary>
    public partial class UptimeDownstreamMessage : UserControl
    {
        public UptimeDownstreamMessage()
        {
            InitializeComponent();
        }





        public string MessageTime
        {
            get { return (string)GetValue(MessageTimeProperty); }
            set { SetValue(MessageTimeProperty, value); }
        }

        public static readonly DependencyProperty MessageTimeProperty =
            DependencyProperty.Register("MessageTime", typeof(string), typeof(UptimeDownstreamMessage), new PropertyMetadata(string.Empty));



        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register("DeviceName", typeof(string), typeof(UptimeDownstreamMessage), new PropertyMetadata(string.Empty));




        public string MessageContent
        {
            get { return (string)GetValue(MessageContentProperty); }
            set { SetValue(MessageContentProperty, value); }
        }

        public static readonly DependencyProperty MessageContentProperty =
            DependencyProperty.Register("MessageContent", typeof(string), typeof(UptimeDownstreamMessage), new PropertyMetadata(string.Empty));




        public Visibility DeviceNameVisibility
        {
            get { return (Visibility)GetValue(DeviceNameVisibilityProperty); }
            set { SetValue(DeviceNameVisibilityProperty, value); }
        }

        public static readonly DependencyProperty DeviceNameVisibilityProperty =
            DependencyProperty.Register("DeviceNameVisibility", typeof(Visibility), typeof(UptimeDownstreamMessage), new PropertyMetadata(Visibility.Collapsed));




    }
}
