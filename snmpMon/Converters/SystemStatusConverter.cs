using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace snmpMon.Converters
{
    public class SystemStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool status)
            {
                string sysStatus = string.Empty;
                switch (status)
                {
                    case true:
                        sysStatus = "Ok";
                        break;
                    case false:
                        sysStatus = "Unreachable";
                        break;

                }
                return sysStatus;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
