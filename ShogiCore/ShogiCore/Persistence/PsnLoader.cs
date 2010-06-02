using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Persistence
{
  public class PsnLoader
  {
//    private readonly Board _board;

    public PsnLoader()
    {
//      _board = new Board(new StandardPieceSet());
//      _board.LoadSnapshotWithoutHistory(BoardSnapshot.InitialPosition);
    }

    public BoardSnapshot Load(GameTrascription trascription)
    {
      var movesB = new AmbiguousMovesSequencesLoader(
        BoardSnapshot.InitialPosition, 
        trascription.Moves, 
        CuteNotation.Instance);
      var _board = movesB.Start();
//      _board.White.Name = trascription.Properties["White"].Value;
//      _board.Black.Name = trascription.Properties["Black"].Value;
      return _board;
    }

  }
}
