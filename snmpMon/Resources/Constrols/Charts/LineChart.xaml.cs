using LiveCharts.Wpf;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Interaction logic for LineChart.xaml
    /// </summary>
    public partial class LineChart : UserControl
    {
        public LineChart()
        {
            InitializeComponent();
        }


        public LegendPosition LegPosition
        {
            get { return (LegendPosition)GetValue(LegPositionProperty); }
            set { SetValue(LegPositionProperty, value); }
        }

        public static readonly DependencyProperty LegPositionProperty =
            DependencyProperty.Register("LegPosition", typeof(LegendPosition), typeof(LineChart), new PropertyMetadata(LegendPosition.Hidden));




        public Paint LegendTextPaint
        {
            get { return (Paint)GetValue(LegendTextPaintProperty); }
            set { SetValue(LegendTextPaintProperty, value); }
        }

        public static readonly DependencyProperty LegendTextPaintProperty =
            DependencyProperty.Register("LegendTextPaint", typeof(Paint), typeof(LineChart), new PropertyMetadata(default));



        public string HighValue
        {
            get { return (string)GetValue(HighValueProperty); }
            set { SetValue(HighValueProperty, value); }
        }

        public static readonly DependencyProperty HighValueProperty =
            DependencyProperty.Register("HighValue", typeof(string), typeof(LineChart), new PropertyMetadata(string.Empty));



        public Visibility HighValueVisibility
        {
            get { return (Visibility)GetValue(HighValueVisibilityProperty); }
            set { SetValue(HighValueVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HighValueVisibilityProperty =
            DependencyProperty.Register("HighValueVisibility", typeof(Visibility), typeof(LineChart), new PropertyMetadata(Visibility.Hidden));




        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LineChart), new PropertyMetadata(string.Empty));




        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(LineChart), new PropertyMetadata(default));


        public ObservableCollection<ISeries> SeriesViews    
        {
            get { return (ObservableCollection<ISeries>)GetValue(SeriesViewsProperty); }
            set { SetValue(SeriesViewsProperty, value); }
        }

        public static readonly DependencyProperty SeriesViewsProperty =
            DependencyProperty.Register("SeriesViews", typeof(ObservableCollection<ISeries>), typeof(LineChart), new PropertyMetadata(default));



        public ObservableCollection<LiveChartsCore.SkiaSharpView.Axis> XAxes
        {
            get { return (ObservableCollection<LiveChartsCore.SkiaSharpView.Axis>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register("XAxes", typeof(ObservableCollection<LiveChartsCore.SkiaSharpView.Axis>), typeof(LineChart), new PropertyMetadata(default));


        public ObservableCollection<Axis> YAxes
        {
            get { return (ObservableCollection<Axis>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
        }

        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register("YAxes", typeof(ObservableCollection<Axis>), typeof(LineChart), new PropertyMetadata(default));


    }
}
