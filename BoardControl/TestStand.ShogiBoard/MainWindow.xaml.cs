using Yasc.ShogiCore;
using Yasc.ShogiCore.Core;

namespace TestStand.ShogiBoard
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      var board = new Board();
      Shogi.InitBoard(board);
      DataContext = board;
    }
  }
}
