using System;
using System.Globalization;
using System.Windows.Data;
using System.Linq;

namespace Yasc.Common
{
  public class AndConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (var value in values.Cast<bool>()) 
        if (!value) 
          return false;
      return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}