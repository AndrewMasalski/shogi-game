using System.Windows;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace Yasc
{
  public partial class ShogiBoardCore
  {
    public ShogiBoardCore()
    {
      InitializeComponent();
    }

    public Board Board
    {
      get { return (Board) DataContext;}
    }

    private void DropHandler(object sender, DropEventArgs e)
    {
      MoveBase move = null;
      if (e.DragSource.DataContext is Cell)
      {
        var from = (Cell)e.DragSource.DataContext;
        var to = (Cell)e.DragTarget.DataContext;
        var m1 = Board.GetUsualMove(from.Position, to.Position, false);
        var m2 = Board.GetUsualMove(from.Position, to.Position, true);
        if (m1.IsValid && m2.IsValid)
        {
          var answer = MessageBox.Show("Promote?", "Q",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
          move = answer == MessageBoxResult.Yes ? m2 : m1;
        }
        else if (m1.IsValid) move = m1;
        else if (m2.IsValid) move = m2;
      }
      else
      {
        var piece = (Piece)e.DragSource.DataContext;
        var to = (Cell)e.DragTarget.DataContext;
        move = Board.GetDropMove(piece, to.Position);
      }

      if (move != null && move.IsValid) 
        Board.MakeMove(move);
    }
  }
}
