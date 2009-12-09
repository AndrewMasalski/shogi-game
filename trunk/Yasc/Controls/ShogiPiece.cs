using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using MvvmFoundation.Wpf;
using Yasc.Controls.Automation;
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
    public ShogiPiece()
    {
    }
    public ShogiPiece(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      Piece = piece;
    }

    #region ' Piece Property '

    public static readonly DependencyProperty PieceProperty =
      DependencyProperty.Register("Piece", typeof(Piece),
        typeof(ShogiPiece), new UIPropertyMetadata(default(Piece), OnPieceChanged));

    private static void OnPieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiPiece)d).OnPieceChanged((Piece)e.NewValue);
    }

    private void OnPieceChanged(Piece piece)
    {
      if (_pieceObserver != null || piece == null)
      {
        throw new InvalidOperationException(
          "You can set ShogiPiece.Piece property just "+
          "once and with not null value only");
      }

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
      get { return (Piece)GetValue(PieceProperty); }
      set { SetValue(PieceProperty, value); }
    }

    #endregion

    #region ' IsFlipped Property '

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.
      IsFlippedProperty.AddOwner(typeof(ShogiPiece), new FrameworkPropertyMetadata(
        false, FrameworkPropertyMetadataOptions.Inherits, OnIsFlippedChanged));

    private static void OnIsFlippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiPiece)d).OnIsFlippedChanged((bool)e.NewValue);
    }

    private void OnIsFlippedChanged(bool value)
    {
      EffectiveDirection = GetEffectiveDirection(value, PieceColor);
    }

    public bool IsFlipped
    {
      get { return (bool)GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    #endregion

    #region ' PieceType Property '

    public static readonly DependencyProperty PieceTypeProperty =
      DependencyProperty.Register("PieceType", typeof(PieceType),
        typeof(ShogiPiece), new UIPropertyMetadata(default(PieceType)));

    public PieceType PieceType
    {
      get { return (PieceType)GetValue(PieceTypeProperty); }
      set { SetValue(PieceTypeProperty, value); }
    }

    #endregion

    #region ' PieceColor Property '

    public static readonly DependencyProperty PieceColorProperty =
      DependencyProperty.Register("PieceColor", typeof(PieceColor),
      typeof(ShogiPiece), new UIPropertyMetadata(PieceColor.White, OnPieceColorChanged));


    public PieceColor PieceColor
    {
      get { return (PieceColor)GetValue(PieceColorProperty); }
      set { SetValue(PieceColorProperty, value); }
    }

    private static void OnPieceColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiPiece)d).OnPieceColorChanged((PieceColor)e.NewValue);
    }

    protected virtual void OnPieceColorChanged(PieceColor pieceColor)
    {
      EffectiveDirection = GetEffectiveDirection(IsFlipped, pieceColor);
    }
    #endregion

    #region ' IsPromoted Property '

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

    public bool IsPromoted
    {
      get { return (bool)GetValue(IsPromotedProperty); }
      set { SetValue(IsPromotedProperty, value); }
    }

    #endregion

    #region ' EffectiveDirection Property '

    public static readonly DependencyProperty EffectiveDirectionProperty =
      DependencyProperty.Register("EffectiveDirection", typeof(PieceDirection),
        typeof(ShogiPiece), new UIPropertyMetadata(PieceDirection.Downwards));

    public PieceDirection EffectiveDirection
    {
      get { return (PieceDirection)GetValue(EffectiveDirectionProperty); }
      set { SetValue(EffectiveDirectionProperty, value); }
    }

    protected static PieceDirection GetEffectiveDirection(bool isFlipped, PieceColor color)
    {
      switch (color)
      {
        case PieceColor.White:
          return isFlipped ? PieceDirection.Upwards : PieceDirection.Downwards;
        case PieceColor.Black:
          return isFlipped ? PieceDirection.Downwards : PieceDirection.Upwards;
      }
      throw new ArgumentOutOfRangeException("color");
    }

    #endregion

    #region ' Root Fields '

    /// <summary>Holds the reference to prevent GC from collecting</summary>
    private PropertyObserver<Piece> _pieceObserver;

    #endregion

    public override string ToString()
    {
      return Piece.ToLatinString();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new ShogiPieceAutomationPeer(this);
    }
  }
}
