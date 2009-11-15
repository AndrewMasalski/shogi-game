using System;
using Yasc.Networking;
using Yasc.ShogiCore.Utils;

namespace Yasc.AI
{
  public class AiController : IPlayerGameController
  {
    #region Implementation of IPlayerGameController

    public IServerGame Game
    {
      get { throw new NotImplementedException(); }
    }

    public PieceColor MyColor
    {
      get { throw new NotImplementedException(); }
    }

    public TimeSpan TimeLeft
    {
      get { throw new NotImplementedException(); }
    }

    public void Move(MoveMsg move)
    {
      throw new NotImplementedException();
    }

    public void Say(string move)
    {
      throw new NotImplementedException();
    }

    public Func<MoveMsg, DateTime> OpponentMadeMove
    {
      set { throw new NotImplementedException(); }
    }

    public event Action<string> OpponentSaidSomething;

    #endregion
  }
}