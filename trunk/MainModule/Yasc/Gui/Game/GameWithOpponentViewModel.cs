using System;
using System.Windows;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.Utils;

namespace Yasc.Gui.Game
{
  public abstract class GameWithOpponentViewModel : GameViewModel
  {
    private readonly Flag _opponentMoveReaction = new Flag();
    public GameTicket Ticket { get; private set; }

    protected override void Init(IPlayerGameController ticket)
    {
      Ticket = new GameTicket(ticket, OnOpponentMadeMove);

      IsFlipped = Ticket.MyColor == PieceColor.Black;
      IsItMyMove = Ticket.MyColor == PieceColor.White;
      IsItOpponentMove = Ticket.MyColor != PieceColor.White;
      base.Init(ticket);
    }
    protected override void BoardOnMoved(object sender, MoveEventArgs args)
    {
      if (!_opponentMoveReaction)
      {
        OnMyMove(args);
      }
      base.BoardOnMoved(sender, args);
    }
    private void OnMyMove(MoveEventArgs args)
    {
      if (Ticket == null) return;
      Ticket.Move(new MoveMsg(args.Move.ToString()));
      if (Board.CurrentSnapshot.IsMateFor(Opponent(Ticket.MyColor)))
      {
        MessageBox.Show("You won!");
      }
    }
    private RelayCommand _sendMessageCommand;

    public ICommand SendMessageCommand
    {
      get
      {
        if (_sendMessageCommand == null)
        {
          _sendMessageCommand = new RelayCommand(SendMessage);
        }
        return _sendMessageCommand;
      }
    }
    private DateTime OnOpponentMadeMove(MoveMsg move)
    {
      using (_opponentMoveReaction.Set())
        Board.MakeMove(Board.GetMove(move.Move));
      return DateTime.Now;
    }
    private void SendMessage()
    {
      MovesAndComments.Add(new ChatMessage(DateTime.Now, CurrentMessage, Ticket.Me.Name));
      CurrentMessage = "";

    }
  }
}