using System;
using System.Globalization;
using System.Windows.Data;

namespace Yasc.Common
{
  public class DirectionToBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (TimerDirection) value == TimerDirection.Forward ? true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (bool)value ? TimerDirection.Forward : TimerDirection.Backward;
    }
  }
}