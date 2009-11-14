using System;
using MvvmFoundation.Wpf;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.Gui
{
  public class GameViewModel : ObservableObject
  {
    private IPlayerGameController _ticket;
    private readonly Flag _opponentMoveReaction = new Flag();

    public Board Board { get; private set; }

    public event EventHandler GameOver;

    private void OnGameOver(EventArgs e)
    {
      var handler = GameOver;
      if (handler != null) handler(this, e);
    }

    public GameViewModel(WelcomeChoice choice)
    {
      switch (choice)
      {
        case WelcomeChoice.ArtificialIntelligence:
          Init(new AIController());
          break;
        case WelcomeChoice.Autoplay:
          InitBoard();
          break;
      }
    }

    public GameViewModel(IPlayerGameController ticket)
    {
      Init(ticket);
    }

    private void Init(IPlayerGameController ticket)
    {
      _ticket = ticket;
      _ticket.OpponentMadeMove = new FuncListener<MoveMsg, DateTime>(TicketOnOpponentMadeMove);

      InitBoard();
    }

    private void InitBoard()
    {
      Board = new Board();
      Board.Move += BoardOnMove;
      Shogi.InititBoard(Board);
    }

    private void BoardOnMove(object sender, MoveEventArgs args)
    {
      if (!_opponentMoveReaction && _ticket != null)
        _ticket.Move(new MoveMsg(args.Move.ToString()));
    }

    private DateTime TicketOnOpponentMadeMove(MoveMsg move)
    {
      using (_opponentMoveReaction.Set())
        Board.MakeMove(Board.GetMove(move.Move));
      return DateTime.Now;
    }
  }

  public class AIController : IPlayerGameController
  {
    #region Implementation of IPlayerGameController

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

    #endregion
  }
}
