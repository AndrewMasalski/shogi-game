﻿using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;
using Yasc.GenericDragDrop;

namespace Yasc.Controls
{
  public class ShogiPiece : Control
  {
    static ShogiPiece()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiPiece),
        new FrameworkPropertyMetadata(typeof(ShogiPiece)));
    }
    
    public static readonly DependencyProperty PieceTypeProperty =
      DependencyProperty.Register("PieceType", typeof(PieceType),
      typeof(ShogiPiece), new UIPropertyMetadata(PieceType.歩));

    public PieceType PieceType
    {
      get { return (PieceType) GetValue(PieceTypeProperty); }
      set { SetValue(PieceTypeProperty, value); }
    }
    
    public static readonly DependencyProperty PieceColorProperty =
      DependencyProperty.Register("PieceColor", typeof(PieceColor),
      typeof(ShogiPiece), new UIPropertyMetadata(PieceColor.White));

    public PieceColor PieceColor
    {
      get { return (PieceColor) GetValue(PieceColorProperty); }
      set { SetValue(PieceColorProperty, value); }
    }

    public override string ToString()
    {
      return PieceColor + (PieceType.IsPromoted ? " promoted " : " ") + PieceType;
    }

    public static readonly DependencyProperty IsFlippedProperty =
      DependencyProperty.Register("IsFlipped", typeof (bool),
        typeof (ShogiPiece), new UIPropertyMetadata(false));

    public bool IsFlipped
    {
      get { return (bool) GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    public ShogiPiece()
    {
      Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
      var ancestor = this.FindAncestor<ShogiBoard>();
      IsFlipped = ancestor.IsFlipped;
    }
  }
}
