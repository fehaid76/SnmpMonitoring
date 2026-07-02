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
    /// Interaction logic for SystemInformation.xaml
    /// </summary>
    public partial class SystemInformation : UserControl
    {
        public SystemInformation()
        {
            InitializeComponent();
        }



        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(SystemInformation), new PropertyMetadata(string.Empty));



        public string SystemName
        {
            get { return (string)GetValue(SystemNameProperty); }
            set { SetValue(SystemNameProperty, value); }
        }

        public static readonly DependencyProperty SystemNameProperty =
            DependencyProperty.Register("SystemName", typeof(string), typeof(SystemInformation), new PropertyMetadata(string.Empty));



        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(string), typeof(SystemInformation), new PropertyMetadata(string.Empty));



        public TimeSpan UpTime
        {
            get { return (TimeSpan)GetValue(UpTimeProperty); }
            set { SetValue(UpTimeProperty, value); }
        }

        public static readonly DependencyProperty UpTimeProperty =
            DependencyProperty.Register("UpTime", typeof(TimeSpan), typeof(SystemInformation), new PropertyMetadata(default));



    }
}
