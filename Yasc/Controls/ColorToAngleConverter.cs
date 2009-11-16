using System;
using System.Globalization;
using System.Windows.Data;
using Yasc.ShogiCore.Utils;

namespace Yasc.Controls
{
  public class ColorToAngleConverter : IValueConverter
  {
    #region Implementation of IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch ((PieceColor)value)
      {
        case PieceColor.Black:
          return 0;
        case PieceColor.White:
          return 180;
      }
      throw new InvalidCastException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}