using System;
using System.Globalization;
using System.Windows.Data;
using Yasc.ShogiCore;

namespace Yasc.Gui
{
  public class ToStirngConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (string)((PieceType)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

}