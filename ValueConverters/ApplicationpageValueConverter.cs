using Restaurant_Pos.Pages;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Restaurant_Pos
{
    /// <summary>
    /// convert the <see cref="ApplicationPage"/> to an actual vew/page
    /// </summary>
    public class ApplicationpageValueConverter : BaseValueConverter<ApplicationpageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Login:
                    return new ServerConfig();

                case ApplicationPage.Retail:
                    return new POSsystem();

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}