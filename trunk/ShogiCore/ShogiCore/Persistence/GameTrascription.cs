using System.Collections.Generic;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Persistence
{
  public class GameTrascription
  {
    public Dictionary<string, TrascriptionProperty> Properties { get; private set; }

    public bool IsFull
    {
      get { return Properties.Count > 0 && Moves.Count > 0; }
    }

    public List<string> Moves { get; private set; }

    public GameTrascription()
    {
      Properties = new Dictionary<string, TrascriptionProperty>();
      Moves = new List<string>();
    }

    public void AddProperty(TrascriptionProperty property)
    {
      Properties[property.Name] = property;
    }

    public BoardSnapshot LoadSnapshot()
    {
      var loader = new AmbiguousMoveSequencesLoader(
        BoardSnapshot.InitialPosition,
        Moves,
        CuteNotation.Instance);
      return loader.Load();
    }
    public Board LoadBoard(IPieceSet pieceSet)
    {
      var board = new Board(pieceSet);
      board.LoadSnapshotWithHistory(LoadSnapshot());
      board.White.Name = Properties["White"].Value;
      board.Black.Name = Properties["Black"].Value;
      return board;

    }
  }
}