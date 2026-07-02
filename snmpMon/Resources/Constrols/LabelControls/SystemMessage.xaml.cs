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

        // Using a DependencyProperty as the backing store for AlertTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlertTimeProperty =
            DependencyProperty.Register("AlertTime", typeof(string), typeof(SystemMessage), new PropertyMetadata(string.Empty));















    }
}
