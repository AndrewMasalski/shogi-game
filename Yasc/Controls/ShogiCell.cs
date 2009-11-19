using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;

namespace Yasc.Controls
{
  public class ShogiCell : Control
  {
    static ShogiCell()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiCell),
        new FrameworkPropertyMetadata(typeof(ShogiCell)));
    }

    public static readonly DependencyProperty IsPossibleMoveTargetProperty =
      DependencyProperty.Register("IsPossibleMoveTarget", typeof(bool),
      typeof(ShogiCell), new UIPropertyMetadata(false));

    public bool IsPossibleMoveTarget
    {
      get { return (bool)GetValue(IsPossibleMoveTargetProperty); }
      set { SetValue(IsPossibleMoveTargetProperty, value); }
    }

    public static readonly DependencyProperty PieceProperty =
      DependencyProperty.Register("Piece", typeof(Piece),
      typeof(ShogiCell), new UIPropertyMetadata(null));

    public Piece Piece
    {
      get { return (Piece)GetValue(PieceProperty); }
      set { SetValue(PieceProperty, value); }
    }

    public static readonly DependencyProperty IsMoveSourceProperty =
      DependencyProperty.Register("IsMoveSource", typeof (bool),
                                  typeof (ShogiCell), new UIPropertyMetadata(default(bool)));

    public bool IsMoveSource
    {
      get { return (bool) GetValue(IsMoveSourceProperty); }
      set { SetValue(IsMoveSourceProperty, value); }
    }
    
  }
}
