using Yasc.ShogiCore;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Snapshots;

namespace TestStand.ShogiBoard
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      var board = new Board();
      board.LoadSnapshot(BoardSnapshot.InitialPosition);
      DataContext = board;
    }
  }
}
