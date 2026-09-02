using Microsoft.Extensions.DependencyInjection;
using snmpMon.ViewModels;
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

namespace snmpMon.Resources.Constrols.Devices
{
    /// <summary>
    /// Interaction logic for DeviceVerification.xaml
    /// </summary>
    public partial class DeviceVerification : UserControl
    {
        public DeviceVerification()
        {
            InitializeComponent();
            
            this.DataContext = App.serviceProvider.GetRequiredService<VerificationViewModel>();

        }









        public string IpAddress
        {
            get { return (string)GetValue(IpAddressProperty); }
            set { SetValue(IpAddressProperty, value); }
        }

        public static readonly DependencyProperty IpAddressProperty =
            DependencyProperty.Register("IpAddress", typeof(string), typeof(DeviceVerification), new FrameworkPropertyMetadata(string.Empty, OnIPAddressChanged));

        private static void OnIPAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DeviceVerification deviceVer)
            {
                if (deviceVer.DataContext is VerificationViewModel vm)
                {
                    vm.IpAddress = e.NewValue?.ToString();
                }
                else
                {
                    deviceVer.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (deviceVer.DataContext is VerificationViewModel vm)
                        {
                            vm.IpAddress = e.NewValue?.ToString();
                        }
                    }));
                }
            }
        }





        public string CommunityString
        {
            get { return (string)GetValue(CommunityStringProperty); }
            set { SetValue(CommunityStringProperty, value); }
        }

        public static readonly DependencyProperty CommunityStringProperty =
            DependencyProperty.Register("CommunityString", typeof(string), typeof(DeviceVerification), new PropertyMetadata(string.Empty, OnCommunityStringChanged));

        private static void OnCommunityStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DeviceVerification deviceVer)
            {
                if (deviceVer.DataContext is VerificationViewModel vm)
                {
                    vm.CommunityString = e.NewValue?.ToString();
                }
                else
                {
                    deviceVer.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (deviceVer.DataContext is VerificationViewModel vm)
                        {
                            vm.CommunityString = e.NewValue?.ToString();
                        }
                    }));
                }
            }
        }
    }
}
