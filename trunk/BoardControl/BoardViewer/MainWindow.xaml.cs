using System;
using System.Windows.Controls;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.BoardViewer
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      _board.Board = new Board(new StandardPieceSet());
    }

    private void SfenChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        _board.Board.LoadSnapshotWithoutHistory(BoardSnapshot.ParseSfen(_sfen.Text));
      }
      catch (ArgumentOutOfRangeException x)
      {
        Console.WriteLine(x);
      }
      catch (NotEnoughPiecesInSetException x)
      {
        Console.WriteLine(x);
      }
      catch (InvalidOperationException x)
      {
        Console.WriteLine(x);
      }
    }
  }
}
