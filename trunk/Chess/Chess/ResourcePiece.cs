using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Chess
{
  public class ResourcePiece : ContentControl
  {
    public PieceType PieceType
    {
      get { return (PieceType) GetValue(PieceTypeProperty); }
      set { SetValue(PieceTypeProperty, value); }
    }

    public static readonly DependencyProperty PieceTypeProperty =
      DependencyProperty.Register("PieceType", typeof (PieceType), typeof (ResourcePiece),
      new PropertyMetadata(default(PieceType)));

    public Uri ResourceUri
    {
      get { return (Uri) GetValue(ResourceUriProperty); }
      set { SetValue(ResourceUriProperty, value); }
    }

    public static readonly DependencyProperty ResourceUriProperty =
      DependencyProperty.Register("ResourceUri", typeof (Uri), typeof (ResourcePiece),
      new PropertyMetadata(default(Uri), PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var resourcePiece = ((ResourcePiece)dependencyObject);
      var uri = new Uri(e.NewValue + "/" + Str(resourcePiece.PieceType) + ".xaml", UriKind.Relative);
      try
      {
        var el = Application.LoadComponent(uri) as FrameworkElement;
        resourcePiece.Content = el;
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
      }
    }
    
    private static string Str(PieceType type)
    {
      switch (type)
      {
        case PieceType.WhitePawn: return "White Pawn";
        case PieceType.WhiteBishop: return "White Bishop";
        case PieceType.WhiteKnight: return "White Knight";
        case PieceType.WhiteRook: return "White Rook";
        case PieceType.WhiteQueen: return "White Queen";
        case PieceType.WhiteKing: return "White King";
        case PieceType.BlackPawn: return "Black Pawn";
        case PieceType.BlackBishop: return "Black Bishop";
        case PieceType.BlackKnight: return "Black Knight";
        case PieceType.BlackRook: return "Black Rook";
        case PieceType.BlackQueen: return "Black Queen";
        case PieceType.BlackKing: return "Black King";
      }
      throw new NotImplementedException();
    }
    
  }

  public enum PieceType
  {
    WhitePawn,
    WhiteBishop,
    WhiteKnight,
    WhiteRook,
    WhiteQueen,
    WhiteKing,

    BlackPawn,
    BlackBishop,
    BlackKnight,
    BlackRook,
    BlackQueen,
    BlackKing,
  }
}