using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace FillingStation.Converters
{
    public abstract class BaseConverter<TFrom, TTo> : MarkupExtension, IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TFrom)value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TTo) value, targetType, parameter, culture);
        }

        public abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);
        public abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}