using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace Yasc.Diagram
{
  public class BoardDiagram : Control
  {
    static BoardDiagram()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BoardDiagram), new FrameworkPropertyMetadata(typeof(BoardDiagram)));
    }

    public static readonly DependencyProperty WhiteProperty =
      DependencyProperty.Register("White", typeof (string),
        typeof (BoardDiagram), new UIPropertyMetadata(null, OnWhiteChanged));

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
        typeof(BoardDiagram), new UIPropertyMetadata(new Board()));

    private static void OnWhiteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoardDiagram) d).OnWhiteChanged((string) e.NewValue);

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
      get { return (string) GetValue(WhiteProperty); }
      set { SetValue(WhiteProperty, value); }
    }
    public string Black
    {
      get { return (string) GetValue(BlackProperty); }
      set { SetValue(BlackProperty, value); }
    }
    public string WhiteHand
    {
      get { return (string) GetValue(WhiteHandProperty); }
      set { SetValue(WhiteHandProperty, value); }
    }
    public string BlackHand
    {
      get { return (string) GetValue(BlackHandProperty); }
      set { SetValue(BlackHandProperty, value); }
    }
    public Board Board
    {
      get { return (Board) GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }

    private void OnWhiteChanged(string pcs)
    {
      ClearBoard(PieceColor.White);
      
      foreach (var pair in ParseB(pcs))
        Board[pair.Key] = new Piece(Board.White, pair.Value);
    }
    private void OnBlackChanged(string pcs)
    {
      ClearBoard(PieceColor.Black);

      foreach (var pair in ParseB(pcs))
        Board[pair.Key] = new Piece(Board.Black, pair.Value);
    }

    private void ClearBoard(PieceColor color)
    {
      foreach (var p in Position.OnBoard)
      {
        if (Board[p] != null && Board[p].Color == color)
          Board[p] = null;
      }
    }

    private void OnWhiteHandChanged(string pcs)
    {
      Board.White.Hand.Clear();

      foreach (var pieceType in ParseA(pcs))
        Board.White.Hand.Add(new Piece(Board.White, pieceType));
    }
    private void OnBlackHandChanged(string pcs)
    {
      Board.Black.Hand.Clear();

      foreach (var pieceType in ParseA(pcs))
        Board.Black.Hand.Add(new Piece(Board.Black, pieceType));
    }
    private static IEnumerable<PieceType> ParseA(string pcs)
    {
      foreach (var p in pcs.Split(','))
        yield return p.Trim();
    }
    private static IEnumerable<KeyValuePair<Position, PieceType>> ParseB(string pcs)
    {
      foreach (var p in pcs.Split(','))
      {
        var trim = p.Trim();
        yield return new KeyValuePair<Position, PieceType>(
          trim.Substring(trim.Length - 2, 2), trim.Substring(0, trim.Length - 2));
      }
    }
  }
}
