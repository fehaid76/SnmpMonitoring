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

namespace snmpMon.Resources.Constrols.InputControls
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : UserControl
    {
        public TextInput()
        {
            InitializeComponent();
        }



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextInput),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TextInput), new PropertyMetadata(string.Empty));




        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TextInput), new PropertyMetadata(default));



        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextInput), new PropertyMetadata(false));



        public Brush TextInputColor
        {
            get { return (Brush)GetValue(TextInputColorProperty); }
            set { SetValue(TextInputColorProperty, value); }
        }

        public static readonly DependencyProperty TextInputColorProperty =
            DependencyProperty.Register("TextInputColor", typeof(Brush), typeof(TextInput), new PropertyMetadata(Brushes.Black));



        public bool FoucableTextInput
        {
            get { return (bool)GetValue(FoucableTextInputProperty); }
            set { SetValue(FoucableTextInputProperty, value); }
        }

        public static readonly DependencyProperty FoucableTextInputProperty =
            DependencyProperty.Register("FoucableTextInput", typeof(bool), typeof(TextInput), new PropertyMetadata(true));







    }
}
