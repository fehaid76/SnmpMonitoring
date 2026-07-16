using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace snmpMon.Converters
{
    public class DeviceLogoVendorIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imgPath = "";
            if (value?.ToString() == "Cisco") imgPath = "Cisco.png";
            if (value?.ToString() == "Juniper") imgPath = "JuniperLogo.png";

            string uriPath = $"pack://application:,,,/Resources/Imgs/{imgPath}";
            try
            {
                new BitmapImage(new Uri(uriPath));
            }
            catch
            {
                return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
