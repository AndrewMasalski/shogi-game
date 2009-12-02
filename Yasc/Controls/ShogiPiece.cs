using System;
using System.Windows;
using System.Windows.Controls;
using MvvmFoundation.Wpf;
using Yasc.ShogiCore;

namespace Yasc.Controls
{
  public class ShogiPiece : Control
  {
    static ShogiPiece()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiPiece),
        new FrameworkPropertyMetadata(typeof(ShogiPiece)));
    }

    #region PieceProperty

    public static readonly DependencyProperty PieceProperty =
      DependencyProperty.Register("Piece", typeof (Piece),
        typeof (ShogiPiece), new UIPropertyMetadata(default(Piece), OnPieceChanged));

    private static void OnPieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiPiece) d).OnPieceChanged((Piece) e.NewValue);
    }

    private void OnPieceChanged(Piece piece)
    {
      PieceType = piece.Type;
      PieceColor = piece.Color;
      IsPromoted = piece.IsPromoted;

      _pieceObserver = new PropertyObserver<Piece>(piece).
        RegisterHandler(p => p.Type, p => PieceType = p.Type).
        RegisterHandler(p => p.Color, p => PieceColor = p.Color).
        RegisterHandler(p => p.IsPromoted, p => IsPromoted = p.IsPromoted);
    }

    public Piece Piece
    {
      get { return (Piece) GetValue(PieceProperty); }
      set { SetValue(PieceProperty, value); }
    }

    #endregion

    public ShogiPiece()
    {
    }

    public ShogiPiece(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      Piece = piece;
    }

    #region IsFlippedProperty

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.IsFlippedProperty.AddOwner(
      typeof(ShogiPiece), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public bool IsFlipped
    {
      get { return (bool)GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    #endregion

    public override string ToString()
    {
      return Piece.ToLatinString();
    }

    public static readonly DependencyProperty PieceTypeProperty =
        DependencyProperty.Register("PieceType", typeof(PieceType),
        typeof(ShogiPiece), new UIPropertyMetadata(default(PieceType)));

    public PieceType PieceType
    {
      get { return (PieceType)GetValue(PieceTypeProperty); }
      set { SetValue(PieceTypeProperty, value); }
    }

    public static readonly DependencyProperty PieceColorProperty =
      DependencyProperty.Register("PieceColor", typeof(PieceColor),
      typeof(ShogiPiece), new UIPropertyMetadata(PieceColor.White));

    public PieceColor PieceColor
    {
      get { return (PieceColor)GetValue(PieceColorProperty); }
      set { SetValue(PieceColorProperty, value); }
    }

    public static readonly DependencyProperty IsPromotedProperty =
      DependencyProperty.Register("IsPromoted", typeof(bool),
        typeof(ShogiPiece), new UIPropertyMetadata(false, OnIsPromotedChanged));

    private static void OnIsPromotedChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiPiece) o).OnIsPromotedChanged((bool)args.NewValue);
    }

    private void OnIsPromotedChanged(bool value)
    {
      if (Piece == null) return;
      Piece.IsPromoted = value;
    }

// ReSharper disable UnaccessedField.Local
    // Need to prevent GC collect observer
    private PropertyObserver<Piece> _pieceObserver;
// ReSharper restore UnaccessedField.Local

    public bool IsPromoted
    {
      get { return (bool)GetValue(IsPromotedProperty); }
      set { SetValue(IsPromotedProperty, value); }
    }
  }
}
