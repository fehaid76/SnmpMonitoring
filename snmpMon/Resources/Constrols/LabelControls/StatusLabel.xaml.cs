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
    /// Interaction logic for StatusLabel.xaml
    /// </summary>
    public partial class StatusLabel : UserControl
    {
        public StatusLabel()
        {
            InitializeComponent();
        }
        // This reperesent the title
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(StatusLabel), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
            
        }

        public static readonly DependencyProperty CountProperty = 
            DependencyProperty.Register("Count", typeof(string), typeof(StatusLabel), new PropertyMetadata(string.Empty));

        public string Count
        {
            get { return (string)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(StatusLabel), new PropertyMetadata(null));

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }



        public Visibility ButtonRefreshVisibility
        {
            get { return (Visibility)GetValue(ButtonRefreshVisibilityProperty); }
            set { SetValue(ButtonRefreshVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ButtonRefreshVisibilityProperty =
            DependencyProperty.Register("ButtonRefreshVisibility", typeof(Visibility), typeof(StatusLabel), new PropertyMetadata(Visibility.Hidden));



        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(StatusLabel), new PropertyMetadata(default));




    }
}
