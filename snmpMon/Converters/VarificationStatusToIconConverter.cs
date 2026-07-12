using snmpMon.Enums;
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
    public class VarificationStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VarificationStatus status)
            {
                string imgName = "";
                switch (status)
                {
                    case VarificationStatus.Idle:
                        imgName = "inf.png";
                        break;

                    case VarificationStatus.Waiting:
                        imgName = "loading.gif";
                        break;
                    case VarificationStatus.Success:
                        imgName = "success.png";
                        break;
                    case VarificationStatus.Failure:
                        imgName = "failure.gif";
                        break;
                    default:
                        imgName = "inf.png";
                        break;
                }

                string uriPath = $"pack://application:,,,/Resources/Imgs/VerificationIcons/{imgName}";

                try
                {
                    return new BitmapImage(new Uri(uriPath));
                }
                catch
                {
                    return null;
                }
                
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
