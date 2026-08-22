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

namespace snmpMon.Resources.Constrols.LabelControls
{
    /// <summary>
    /// Interaction logic for SystemMessage.xaml
    /// </summary>
    public partial class SystemMessage : UserControl
    {
        public SystemMessage()
        {
            InitializeComponent();
        }



        public string MessageContent
        {
            get { return (string)GetValue(MessageContentProperty); }
            set { SetValue(MessageContentProperty, value); }
        }

        public static readonly DependencyProperty MessageContentProperty =
            DependencyProperty.Register("MessageContent", typeof(string), typeof(SystemMessage), new PropertyMetadata(string.Empty));



        public string SNMPSeverity
        {
            get { return (string)GetValue(SNMPSeverityProperty); } 
            set { SetValue(SNMPSeverityProperty, value); }
        }

        public static readonly DependencyProperty SNMPSeverityProperty =
            DependencyProperty.Register("SNMPSeverity", typeof(string), typeof(SystemMessage), new PropertyMetadata(string.Empty));




        public int SeverityLevel
        {
            get { return (int)GetValue(SeverityLevelProperty); }
            set { SetValue(SeverityLevelProperty, value); }
        }

        public static readonly DependencyProperty SeverityLevelProperty =
            DependencyProperty.Register("SeverityLevel", typeof(int), typeof(SystemMessage), new PropertyMetadata(default));





        public string Device
        {
            get { return (string)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }

        public static readonly DependencyProperty DeviceProperty =
            DependencyProperty.Register("Device", typeof(string), typeof(SystemMessage), new PropertyMetadata(string.Empty));




        public Visibility DeviceNameVisibility
        {
            get { return (Visibility)GetValue(DeviceNameVisibilityProperty); }
            set { SetValue(DeviceNameVisibilityProperty, value); }
        }

        public static readonly DependencyProperty DeviceNameVisibilityProperty =
            DependencyProperty.Register("DeviceNameVisibility", typeof(Visibility), typeof(SystemMessage), new PropertyMetadata(Visibility.Hidden));



        public ImageSource WarningLevelIcon
        {
            get { return (ImageSource)GetValue(WarningLevelIconProperty); }
            set { SetValue(WarningLevelIconProperty, value); }
        }

        public static readonly DependencyProperty WarningLevelIconProperty =
            DependencyProperty.Register("WarningLevelIcon", typeof(ImageSource), typeof(SystemMessage), new PropertyMetadata(null));





        public string AlertTime
        {
            get { return (string)GetValue(AlertTimeProperty); }
            set { SetValue(AlertTimeProperty, value); }
        }

        public static readonly DependencyProperty AlertTimeProperty =
            DependencyProperty.Register("AlertTime", typeof(string), typeof(SystemMessage), new PropertyMetadata(string.Empty));















    }
}
