using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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







        public ObservablePoint[] Labelx
        {
            get { return (ObservablePoint[])GetValue(LabelxProperty); }
            set { SetValue(LabelxProperty, value); }
        }

        public static readonly DependencyProperty LabelxProperty =
            DependencyProperty.Register("Labelx", typeof(ObservablePoint[]), typeof(LinerChart), new PropertyMetadata(default));




        public IList<string> LabelsX
        {
            get { return (IList<string>)GetValue(LabelsXProperty); }
            set { SetValue(LabelsXProperty, value); }
        }

        public static readonly DependencyProperty LabelsXProperty =
            DependencyProperty.Register("LabelsX", typeof(IList<string>), typeof(LinerChart), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));




        public double AxisMin
        {
            get { return (double)GetValue(AxisMinProperty); }
            set { SetValue(AxisMinProperty, value); }
        }

        public static readonly DependencyProperty AxisMinProperty =
            DependencyProperty.Register("AxisMin", typeof(double), typeof(LinerChart), new PropertyMetadata(default));




        public double AxisMax
        {
            get { return (double)GetValue(AxisMaxProperty); }
            set { SetValue(AxisMaxProperty, value); }
        }

        public static readonly DependencyProperty AxisMaxProperty =
            DependencyProperty.Register("AxisMax", typeof(double), typeof(LinerChart), new PropertyMetadata(default));





        public Visibility ClearButtonVisibility
        {
            get { return (Visibility)GetValue(ClearButtonVisibilityProperty); }
            set { SetValue(ClearButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ClearButtonVisibilityProperty =
            DependencyProperty.Register("ClearButtonVisibility", typeof(Visibility), typeof(LinerChart), new PropertyMetadata(Visibility.Hidden));




        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(LinerChart), new PropertyMetadata(default));






    }
}
