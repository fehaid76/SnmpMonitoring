using LiveCharts;
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
    /// Interaction logic for LinerChart.xaml
    /// </summary>
    public partial class LinerChart : UserControl
    {
        public LinerChart()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SeriesColProperty =
            DependencyProperty.Register("SeriesCol", typeof(SeriesCollection), typeof(LinerChart), new PropertyMetadata(default));

        public SeriesCollection SeriesCol
        {
            get { return (SeriesCollection)GetValue(SeriesColProperty); }
            set
            {
                SetValue(SeriesColProperty, value);
            }
        }

        public static readonly DependencyProperty LabelsXProperty =
            DependencyProperty.Register("LabelsX", typeof(List<string>), typeof(LinerChart), new PropertyMetadata(default));

        public List<string> LabelsX
        {
            get {return (List<string>)GetValue(LabelsXProperty);}
            set { SetValue(LabelsXProperty, value); }
        }

        public static readonly DependencyProperty LabelsYProperty =
    DependencyProperty.Register("LabelsY", typeof(List<string>), typeof(LinerChart), new PropertyMetadata(default));

        public List<string> LabelsY
        {
            get { return (List<string>)GetValue(LabelsYProperty); }
            set { SetValue(LabelsYProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LinerChart), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
