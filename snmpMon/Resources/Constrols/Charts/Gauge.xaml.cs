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

namespace snmpMon.Resources.Constrols.Charts
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class Gauge : UserControl
    {
        public Gauge()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Gauge), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty);}
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register("Value", typeof(double), typeof(Gauge), new PropertyMetadata(default));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }

        }

        public static readonly DependencyProperty FromValueProperty =
            DependencyProperty.Register("FromValue", typeof(double), typeof(Gauge), new PropertyMetadata(default));

        public double FromValue
        {
            get { return (double)GetValue(FromValueProperty);}
            set { SetValue(FromValueProperty, value); }

        }

        public static readonly DependencyProperty ToValueProperty =
    DependencyProperty.Register("ToValue", typeof(double), typeof(Gauge), new PropertyMetadata(default));

        public double ToValue
        {
            get { return (double)GetValue(ToValueProperty); }
            set { SetValue(ToValueProperty, value); }

        }








    }
}
