using snmpMon.ViewModels;
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
    public class MessageBoxIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageBoxIcon Icon)
            {
                string imgName = "";
                switch (Icon)
                {
                    case MessageBoxIcon.Information:
                        imgName = "Information.png";
                        break;
                    case MessageBoxIcon.Warning:
                        imgName = "Warning.png";
                        break;
                    case MessageBoxIcon.Error:
                        imgName = "Error.png";
                        break;
                    case MessageBoxIcon.Question:
                        imgName = "Question.png";
                        break;

                }

                string uriPath = $"pack://application:,,,/Resources/Imgs/MessageBoxIcons/{imgName}";

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
