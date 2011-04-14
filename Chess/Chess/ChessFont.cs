using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Chess
{
  public class ChessFont
  {
    private string _mapFrom = "PBNRQKpbnrqk";
    private string _pieceMap = "PBNRQKpbnrqk";
    private FontFamily _fontFamily;
    private double _fontSizeCoef;

    public string MapFrom
    {
      get { return _mapFrom; }
      set { _mapFrom = value; }
    }
    public string PieceMap
    {
      get { return _pieceMap; }
      set
      {
        if (_pieceMap == value) return;
        _pieceMap = value;
        _fontSizeCoef = 0;
      }
    }
    public FontFamily FontFamily
    {
      get { return _fontFamily; }
      set
      {
        if (_fontFamily == value) return;
        _fontFamily = value;
        _fontSizeCoef = 0;
      }
    }

    public double FontSizeCoef
    {
      get
      {
        if (_fontSizeCoef == 0)
        {
          var maxWidth = 0.0;
          var maxHeight = 0.0;
          foreach (var ch in _pieceMap)
          {
            var formattedText = new FormattedText(ch.ToString(),
              CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
              new Typeface(_fontFamily.ToString()), 100, Brushes.Black);

            if (formattedText.Height > maxHeight) maxHeight = formattedText.Height;
            if (formattedText.Width > maxWidth) maxWidth = formattedText.Width;
          }
          _fontSizeCoef = 120 / Math.Max(maxWidth, maxHeight);          
        }
        return _fontSizeCoef;
      }
    }

    public ChessFont()
    {
    }

    public ChessFont(FontFamily fontFamily, string pieceMap = "PBNRQKpbnrqk", string mapFrom = "PBNRQKpbnrqk")
    {
      if (fontFamily == null) throw new ArgumentNullException("fontFamily");
      if (mapFrom == null) throw new ArgumentNullException("mapFrom");
      if (pieceMap == null) throw new ArgumentNullException("pieceMap");

      FontFamily = fontFamily;
      _mapFrom = mapFrom;
      _pieceMap = pieceMap;
    }

    public char GetPiece(char pieceSymbol)
    {
      var indexOf = _mapFrom.IndexOf(pieceSymbol);
      if (indexOf == -1) return '?';
      if (indexOf >= _pieceMap.Length) return 'X';
      return _pieceMap[indexOf];
    }

    public override string ToString()
    {
      var s = FontFamily.ToString();
      return s.Substring(s.IndexOf("#")+1);
    }
  }
}