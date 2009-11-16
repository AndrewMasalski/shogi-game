using System;
using System.Globalization;
using System.Windows.Data;
using Yasc.ShogiCore.Utils;

namespace Yasc.Controls
{
  public class ColorToAngleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (PieceColor) value == PieceColor.White ? 180 : 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
  public class IsFlippedToAngleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (bool) value ? 180 : 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}