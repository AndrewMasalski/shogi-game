using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace Yasc.Controls
{
  public class ShogiHand : Control
  {
    static ShogiHand()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiHand),
        new FrameworkPropertyMetadata(typeof(ShogiHand)));
    }

    public static readonly DependencyProperty BoardProperty =
      DependencyProperty.Register("Board", typeof (Board),
                                  typeof (ShogiHand), new UIPropertyMetadata(default(Board), OnBoardChanged));

    private static void OnBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiHand)d).OnBoardChanged((Board)e.NewValue);
    }

    private void OnBoardChanged(Board board)
    {
      Hand = board[Color].Hand;
    }

    public Board Board
    {
      get { return (Board) GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
      DependencyProperty.Register("Color", typeof (PieceColor),
        typeof (ShogiHand), new UIPropertyMetadata(default(PieceColor), OnColorChanged));

    private static void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiHand)o).OnColorChanged((PieceColor)args.NewValue);
    }

    private void OnColorChanged(PieceColor color)
    {
      Hand = Board == null ? null : Board[color].Hand;
    }

    public PieceColor Color
    {
      get { return (PieceColor) GetValue(ColorProperty); }
      set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty HandProperty =
      DependencyProperty.Register("Hand", typeof (ObservableCollection<Piece>),
                                  typeof (ShogiHand), new UIPropertyMetadata(default(ObservableCollection<Piece>)));

    public ObservableCollection<Piece> Hand
    {
      get { return (ObservableCollection<Piece>) GetValue(HandProperty); }
      set { SetValue(HandProperty, value); }
    }
    
  }
}