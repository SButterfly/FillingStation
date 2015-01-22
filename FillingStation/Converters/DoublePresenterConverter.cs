using System;
using System.Globalization;

namespace FillingStation.Converters
{
    public class DoublePresenterConverter : BaseConverter<double, string>
    {
        public override string Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Math.Round(value, 2).ToString("#,##0.00");

            return FormatPresenterConverter.Instance.Convert(result, targetType, parameter, culture);
        }

        public override double ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}