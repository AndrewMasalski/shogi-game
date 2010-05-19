using Yasc.ShogiCore;
using Yasc.ShogiCore.Core;

namespace Yasc.Persistence
{
  public class PsnLoader
  {
    private readonly Board _board;

    public PsnLoader()
    {
      _board = new Board();
      Shogi.InitBoard(_board);
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
