using System;
using System.Windows;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.AI;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.Utils;

namespace Yasc.Gui
{
  public class GameViewModel : ObservableObject
  {
    #region ' Fields '

    private TimeSpan _opponentTime = TimeSpan.FromSeconds(5);
    private TimeSpan _ourTime = TimeSpan.FromSeconds(5);
    private bool _isFlipped;
    private readonly IServerGame _game;
    private RelayCommand _cleanBoardCommand;
    private RelayCommand _getBackCommand;
    private readonly Flag _opponentMoveReaction = new Flag();
    private bool _isItOpponentMove;
    private bool _isItMyMove;

    #endregion

    #region ' Public Interface '

    /// <summary> </summary>
    public bool IsItMyMove
    {
      get { return _isItMyMove; }
      set
      {
        if (_isItMyMove == value) return;
        _isItMyMove = value;
        RaisePropertyChanged("IsItMyMove");
        if (!value && MyTime == TimeSpan.Zero) return;
        IsItOpponentMove = !value;
      }
    }
    /// <summary> </summary>
    public bool IsItOpponentMove
    {
      get { return _isItOpponentMove; }
      set
      {
        if (_isItOpponentMove == value) return;
        _isItOpponentMove = value;
        RaisePropertyChanged("IsItOpponentMove");
        if (!value && OpponentTime == TimeSpan.Zero) return;
        IsItMyMove = !value;
      }
    }

    public GameTicket Ticket { get; private set; }
    public bool IsFlipped
    {
      get { return _isFlipped; }
      set
      {
        if (_isFlipped == value) return;
        _isFlipped = value;
        RaisePropertyChanged("IsFlipped");
      }
    }
    /// <summary>Time application user has left</summary>
    public TimeSpan MyTime
    {
      get { return _ourTime; }
      set
      {
        if (_ourTime == value) return;
        _ourTime = value;
        RaisePropertyChanged("MyTime");
      }
    }
    /// <summary>Time user's opponent has left</summary>
    public TimeSpan OpponentTime
    {
      get { return _opponentTime; }
      set
      {
        if (_opponentTime == value) return;
        _opponentTime = value;
        RaisePropertyChanged("OpponentTime");
      }
    }
    public Board Board { get; private set; }
    public ICommand CleanBoardCommand
    {
      get
      {
        if (_cleanBoardCommand == null)
        {
          _cleanBoardCommand = new RelayCommand(CleanBoard);
        }
        return _cleanBoardCommand;
      }
    }
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

    public GameViewModel(WelcomeChoice choice)
    {
      switch (choice)
      {
        case WelcomeChoice.ArtificialIntelligence:
          Init(new CleverAiController());
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

    public event EventHandler GameOver;

    #endregion

    #region ' Implementation '

    private void Init(IPlayerGameController ticket)
    {
      Ticket = new GameTicket(ticket, OnOpponentMadeMove);

      IsFlipped = Ticket.MyColor == PieceColor.White;
      InitBoard();
    }
    private void InitBoard()
    {
      Board = new Board();
      Board.Moved += BoardOnMoved;
      Shogi.InitBoard(Board);
    }
    private void GetBack()
    {
      OnGameOver(EventArgs.Empty);
    }
    private void OnGameOver(EventArgs e)
    {
      var handler = GameOver;
      if (handler != null) handler(this, e);
    }
    private void CleanBoard()
    {
      foreach (var cell in Board.Cells)
      {
        var piece = cell.Piece;
        if (piece != null)
        {
          var player = piece.Owner;
          cell.ResetPiece();
          player.Hand.Add(piece);
        }
      }
    }
    private void BoardOnMoved(object sender, MoveEventArgs args)
    {
      // If it's not opponent than it must be me
      if (!_opponentMoveReaction && Ticket != null)
      {
        Ticket.Move(new MoveMsg(args.Move.ToString()));
        if (Board.CurrentSnapshot.IsMateFor(Opponent(Ticket.MyColor)))
        {
          MessageBox.Show("You won!");
        }
      }
      IsItMyMove = !IsItMyMove;
    }
    private static PieceColor Opponent(PieceColor color)
    {
      return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
    private DateTime OnOpponentMadeMove(MoveMsg move)
    {
      using (_opponentMoveReaction.Set())
        Board.MakeMove(Board.GetMove(move.Move));
      return DateTime.Now;
    }

    #endregion

  }

  public class GameTicket
  {
    private readonly IPlayerGameController _ticket;

    public PieceColor MyColor
    {
      get { return _ticket.MyColor; }
    }

    public IServerUser Me
    {
      get { return _ticket.Game.InviteeColor == MyColor ? _ticket.Game.Invitor : _ticket.Game.Invitee; }
    }
    public IServerUser Opponent
    {
      get { return _ticket.Game.InviteeColor != MyColor ? _ticket.Game.Invitor : _ticket.Game.Invitee; }
    }

    public GameTicket(IPlayerGameController ticket, Func<MoveMsg, DateTime> opponentMadeMoveCallback)
    {
      if (opponentMadeMoveCallback == null)
        throw new ArgumentNullException("opponentMadeMoveCallback");

      _ticket = ticket;
      _ticket.OpponentMadeMove = new FuncListener<MoveMsg, DateTime>(opponentMadeMoveCallback);
    }

    public void Move(MoveMsg msg)
    {
      _ticket.Move(msg);
    }
  }
}
