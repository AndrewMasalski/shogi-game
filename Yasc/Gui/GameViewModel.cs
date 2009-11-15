using System;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.AI;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.Gui
{
  public class GameViewModel : ObservableObject
  {
    private readonly IServerGame _game;
    private IPlayerGameController _ticket;
    private readonly Flag _opponentMoveReaction = new Flag();

    public Board Board { get; private set; }

    public ICommand GetBackCommand
    {
      get
      {
        if (_getBackCommand == null)
        {
          _getBackCommand = new RelayCommand(GetBack);
        }
        return _getBackCommand;
      }
    }

    private void GetBack()
    {
      OnGameOver(EventArgs.Empty);
    }

    private RelayCommand _getBackCommand;

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
          Init(new AiController());
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

    public GameViewModel(IServerGame game)
    {
      _game = game;
    }

    private void Init(IPlayerGameController ticket)
    {
      _ticket = ticket;
      _ticket.OpponentMadeMove = new FuncListener<MoveMsg, DateTime>(OnOpponentMadeMove);

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
      // If it's not opponent than it must be me
      if (!_opponentMoveReaction && _ticket != null)
        _ticket.Move(new MoveMsg(args.Move.ToString()));
    }

    private DateTime OnOpponentMadeMove(MoveMsg move)
    {
      using (_opponentMoveReaction.Set())
        Board.MakeMove(Board.GetMove(move.Move));
      return DateTime.Now;
    }
  }
}
