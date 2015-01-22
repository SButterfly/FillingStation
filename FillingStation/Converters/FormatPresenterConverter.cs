using System;
using System.Globalization;

namespace FillingStation.Converters
{
    public class FormatPresenterConverter : BaseConverter<string, string>
    {
        public override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            var formatStr = parameter as string;
            if (formatStr != null)
            {
                return string.Format(formatStr, value);
            }
            return value;
        }

        public override string ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static FormatPresenterConverter _formatPresenterConverter;
        private static readonly object sync = new object();
        public static FormatPresenterConverter Instance
        {
            get
            {
                if (_formatPresenterConverter == null)
                {
                    lock (sync)
                    {
                        if (_formatPresenterConverter == null) 
                            _formatPresenterConverter = new FormatPresenterConverter();
                    }
                }
                return _formatPresenterConverter;
            }
        }
    }
}