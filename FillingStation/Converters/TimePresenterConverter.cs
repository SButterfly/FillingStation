using System;
using System.Globalization;
using System.Linq;

namespace FillingStation.Converters
{
    public class TimePresenterConverter : BaseConverter<TimeSpan, string>
    {
        public override string Convert(TimeSpan value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = ToCustomFormat(value);
            return FormatPresenterConverter.Instance.Convert(result, targetType, parameter, culture);
        }

        public override TimeSpan ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string ToCustomFormat(TimeSpan timeSpan)
        {
            var str = timeSpan.ToString("g", CultureInfo.InvariantCulture);

            int lastPointIndex = str.LastIndexOf('.');
            if (lastPointIndex >= 0)
            {
                str = str.Substring(0, Math.Min(str.Length, lastPointIndex + 3));
            }

            var values = str.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            var result = "";
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Any(c => c != '0'))
                {
                    for (int j = i; j < values.Length; j++)
                    {
                        result += values[j] + ":";
                    }
                    break;
                }
            }

            result = result.TrimEnd(':');
            result = String.IsNullOrEmpty(result) ? values.Last() : result;
            result = result.All(c => c == '0') ? "0" : result;

            return result;
        }
    }
}