using System;
using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;

namespace Yasc.Controls
{
  public abstract class PieceHolderBase : ContentControl
  {
    #region ' EffectiveDirectionProperty '

    public static readonly DependencyProperty EffectiveDirectionProperty =
      DependencyProperty.Register("EffectiveDirection", typeof(PieceDirection),
        typeof(PieceHolderBase), new UIPropertyMetadata(default(PieceDirection)));

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

    #region ' IsPossibleMoveTarget Property  '

    public static readonly DependencyProperty IsPossibleMoveTargetProperty =
      DependencyProperty.Register("IsPossibleMoveTarget", typeof(bool),
        typeof(PieceHolderBase), new UIPropertyMetadata(false));

    public bool IsPossibleMoveTarget
    {
      get { return (bool)GetValue(IsPossibleMoveTargetProperty); }
      set { SetValue(IsPossibleMoveTargetProperty, value); }
    }

    #endregion

    #region ' IsMoveSource Property '

    public static readonly DependencyProperty IsMoveSourceProperty =
      DependencyProperty.Register("IsMoveSource", typeof(bool),
                                  typeof(PieceHolderBase), new UIPropertyMetadata(false));


    public bool IsMoveSource
    {
      get { return (bool)GetValue(IsMoveSourceProperty); }
      set { SetValue(IsMoveSourceProperty, value); }
    }

    #endregion

    #region ' IsFlipped Property '

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.IsFlippedProperty.AddOwner(
      typeof(PieceHolderBase), new FrameworkPropertyMetadata(false, 
        FrameworkPropertyMetadataOptions.Inherits, OnIsFlippedChanged));

    private static void OnIsFlippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieceHolderBase) d).OnIsFlippedChanged((bool) e.NewValue);
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
    
    static PieceHolderBase()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PieceHolderBase),
         new FrameworkPropertyMetadata(typeof(PieceHolderBase)));
    }
    protected abstract void UpdateHolderControl();

    public virtual ShogiPiece DetachPiece()
    {
      var piece = ShogiPiece;
      Content = null;
      return piece;
    }
    public ShogiPiece ShogiPiece
    {
      get { return (ShogiPiece)Content; }
    }

    #region ' PieceType Property '

    public static readonly DependencyProperty PieceTypeProperty =
      DependencyProperty.Register("PieceType", typeof(PieceType),
        typeof(PieceHolderBase), new UIPropertyMetadata(default(PieceType), OnPieceTypeChanged));

    private static void OnPieceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieceHolderBase)d).OnPieceTypeChanged((PieceType)e.NewValue);
    }

    protected virtual void OnPieceTypeChanged(PieceType pieceType)
    {
      UpdateHolderControl();
    }

    public PieceType PieceType
    {
      get { return (PieceType)GetValue(PieceTypeProperty); }
      set { SetValue(PieceTypeProperty, value); }
    }

    #endregion

    #region ' PieceColor Property '

    public static readonly DependencyProperty PieceColorProperty =
      DependencyProperty.Register("PieceColor", typeof(PieceColor),
        typeof(PieceHolderBase), new UIPropertyMetadata(default(PieceColor), OnPieceColorChanged));

    private static void OnPieceColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieceHolderBase)d).OnPieceColorChanged((PieceColor)e.NewValue);
    }

    protected virtual void OnPieceColorChanged(PieceColor pieceColor)
    {
      EffectiveDirection = GetEffectiveDirection(IsFlipped, pieceColor);
    }

    public PieceColor PieceColor
    {
      get { return (PieceColor)GetValue(PieceColorProperty); }
      set { SetValue(PieceColorProperty, value); }
    }

    #endregion
  }
}