using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Yasc.BoardControl.Controls;
using Yasc.BoardControl.GenericDragDrop;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace Yasc.RulesVisualization
{
  [ContentProperty("Moves")]
  public class BoardDiagram : Control
  {
    static BoardDiagram()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BoardDiagram), new FrameworkPropertyMetadata(typeof(BoardDiagram)));
    }
    public BoardDiagram()
    {
      Moves = new ObservableCollection<MovesBase>();
      Moves.CollectionChanged += (s, e) => UpdateShownMoves();
    }

    #region ' Main Properties '

    public static readonly DependencyProperty WhiteProperty =
      DependencyProperty.Register("White", typeof(string),
        typeof(BoardDiagram), new UIPropertyMetadata(null, OnWhiteChanged));

    public static readonly DependencyProperty BlackProperty =
      DependencyProperty.Register("Black", typeof(string),
        typeof(BoardDiagram), new UIPropertyMetadata(null, OnBlackChanged));

    public static readonly DependencyProperty WhiteHandProperty =
      DependencyProperty.Register("WhiteHand", typeof(string),
        typeof(BoardDiagram), new UIPropertyMetadata(null, OnWhiteHandChanged));

    public static readonly DependencyProperty BlackHandProperty =
      DependencyProperty.Register("BlackHand", typeof(string),
        typeof(BoardDiagram), new UIPropertyMetadata(null, OnBlackHandChanged));

    public static readonly DependencyProperty BoardProperty =
      DependencyProperty.Register("Board", typeof(Board),
        typeof(BoardDiagram), new UIPropertyMetadata(
          new Board(InfinitePieceSet.Instance)));

    private static void OnWhiteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoardDiagram)d).OnWhiteChanged((string)e.NewValue);

    }
    private static void OnBlackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoardDiagram)d).OnBlackChanged((string)e.NewValue);

    }
    private static void OnWhiteHandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoardDiagram)d).OnWhiteHandChanged((string)e.NewValue);

    }
    private static void OnBlackHandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoardDiagram)d).OnBlackHandChanged((string)e.NewValue);

    }

    public string White
    {
      get { return (string)GetValue(WhiteProperty); }
      set { SetValue(WhiteProperty, value); }
    }
    public string Black
    {
      get { return (string)GetValue(BlackProperty); }
      set { SetValue(BlackProperty, value); }
    }
    public string WhiteHand
    {
      get { return (string)GetValue(WhiteHandProperty); }
      set { SetValue(WhiteHandProperty, value); }
    }
    public string BlackHand
    {
      get { return (string)GetValue(BlackHandProperty); }
      set { SetValue(BlackHandProperty, value); }
    }
    public Board Board
    {
      get { return (Board)GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }

    private void OnWhiteChanged(string pcs)
    {
      ClearBoard(PieceColor.White);

      foreach (var pair in ParseB(pcs))
      {
        var p = pair.Key;
        Board.SetPiece(pair.Value, p, Board.White);
      }
    }
    private void OnBlackChanged(string pcs)
    {
      ClearBoard(PieceColor.Black);

      foreach (var pair in ParseB(pcs))
      {
        var p = pair.Key;
        Board.SetPiece(pair.Value, p, Board.Black);
      }   
    }
    private void OnWhiteHandChanged(string pcs)
    {
      Board.White.Hand.Clear();
      foreach (var pieceType in ParseA(pcs))
        Board.White.Hand.Add(pieceType);
    }
    private void OnBlackHandChanged(string pcs)
    {
      Board.Black.Hand.Clear();
      foreach (var pieceType in ParseA(pcs))
        Board.Black.Hand.Add(pieceType);
    }

    #endregion

    private void ClearBoard(PieceColor color)
    {
      foreach (var p in Position.OnBoard)
      {
        var piece = Board.GetPieceAt(p);
        if (piece != null && piece.Color == color)
          Board.ResetPiece(p);
      }
    }
    private static IEnumerable<IPieceType> ParseA(string pcs)
    {
      return pcs.Split(',').Select(p => PT.Parse(p.Trim()));
    }

    private static IEnumerable<KeyValuePair<Position, IPieceType>> ParseB(string pcs)
    {
      foreach (var p in pcs.Split(','))
      {
        var trim = p.Trim();
        yield return new KeyValuePair<Position, IPieceType>(
          Position.Parse(trim.Substring(trim.Length - 2, 2)), 
          PT.Parse(trim.Substring(0, trim.Length - 2)));
      }
    }

    public ObservableCollection<MovesBase> Moves { get; private set; }

    #region ' MovesToShow Property '

    public int MovesToShow
    {
      get { return (int)GetValue(MovesToShowProperty); }
      set { SetValue(MovesToShowProperty, value); }
    }

    public static readonly DependencyProperty MovesToShowProperty =
      DependencyProperty.Register("MovesToShow", typeof(int),
        typeof(BoardDiagram), new UIPropertyMetadata(0, OnMovesToShowChanged));

    private static void OnMovesToShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoardDiagram)d).UpdateShownMoves();
    }

    #endregion

    #region ' ShownMoves '

    private MovesBase _shownMoves;
    private void UpdateShownMoves()
    {
      if (_shownMoves != null) HideMoves();
      _shownMoves = Moves.Count > MovesToShow ? Moves[MovesToShow] : null;
      if (_shownMoves != null) ShowMoves(_shownMoves);
    }
    private void ShowMoves(MovesBase moves)
    {
      var board = this.FindChild<ShogiBoard>();
      if (board != null)
      {
        moves.ShowMoves(board);
      }
    }
    private void HideMoves()
    {
      var board = this.FindChild<ShogiBoard>();
      if (board == null) return;

      ResetFlagsOnBoard(board);
      ResetFlagsInHands(board);
    }

    private static void ResetFlagsInHands(ShogiBoard board)
    {
      foreach (var color in new[] { PieceColor.White, PieceColor.Black })
        foreach (var type in new[]
                               {
                                 PT.王, PT.玉, 
                                 PT.飛, PT.角, 
                                 PT.金, PT.銀, 
                                 PT.桂, PT.香,PT.歩,
                               })
        {
          var nest = board.GetHand(color).GetPiece(type);
          if (nest != null) nest.IsMoveSource = false;
        }
    }

    private static void ResetFlagsOnBoard(ShogiBoard board)
    {
      foreach (var p in Position.OnBoard)
      {
        var cell = board.GetCell(p);
        cell.IsMoveSource = false;
        cell.IsPossibleMoveTarget = false;
      }
    }

    #endregion
  }
}