using System;
using System.Globalization;
using System.Windows.Data;

namespace Yasc.Common
{
  public class ScalarConverter : IValueConverter
  {
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (bool)value != Invert;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (bool) value != Invert;
    }
  }
}