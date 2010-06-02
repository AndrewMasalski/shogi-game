using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Snapshots;

namespace TestStand.ShogiBoard
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      var board = new Board(new StandardPieceSet());
      board.LoadSnapshotWithoutHistory(BoardSnapshot.InitialPosition);
      DataContext = board;
    }
  }
}
