using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.Persistence
{
  public class PsnLoader
  {
    private readonly Board _board;

    public PsnLoader()
    {
      _board = new Board(new StandardPieceSet());
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
    }

    public Board Load(GameTrascription trascription)
    {
      var movesB = new AmbiguousMovesSequencesLoader(_board, trascription.Moves);
      movesB.Start();
      _board.White.Name = trascription.Properties["White"].Value;
      _board.Black.Name = trascription.Properties["Black"].Value;
      return _board;
    }

  }
}
