using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MainModule.Model;
using MainModule.Utils;
using Yasc.Networking.Interfaces;
using Yasc.Networking.Utils;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.Primitives;
using Yasc.Utils;
using Yasc.Utils.Mvvm;

namespace MainModule.Gui.Game
{
  public abstract class GameWithOpponentViewModel : GameViewModel
  {
    private RelayCommand _sendMessageCommand;
    private RelayCommand _takeBackCommand;
    private readonly Flag _opponentMoveReaction = new Flag();

    public ICommand TakeBackCommand
    {
      get { return _takeBackCommand ?? (_takeBackCommand = new RelayCommand(UndoLastMove)); }
    }
    public ICommand SendMessageCommand
    {
      get { return _sendMessageCommand ?? (_sendMessageCommand = new RelayCommand(SendMessage)); }
    }
    public GameTicket Ticket { get; private set; }

    protected void InitTicket(IPlayerGameController ticket)
    {
      Ticket = new GameTicket(ticket, OnOpponentMadeMove);

      IsFlipped = Ticket.MyColor == PieceColor.Black;
      IsItMyMove = Ticket.MyColor == PieceColor.White;
      IsItOpponentMove = Ticket.MyColor != PieceColor.White;
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
      Ticket.Move(new MoveMsg(args.DecoratedMove.ToString()));
      if (Board.CurrentSnapshot.IsMateFor(Opponent(Ticket.MyColor)))
      {
        MessageBox.Show("You won!");
      }
    }
    private DateTime OnOpponentMadeMove(MoveMsg move)
    {
      using (_opponentMoveReaction.Set())
        Board.MakeMove(Board.GetMove(move.Move, FormalNotation.Instance).First());
      return DateTime.Now;
    }
    private void SendMessage()
    {
      MovesAndComments.Add(new ChatMessage(DateTime.Now, CurrentMessage, Ticket.Me.Name));
      CurrentMessage = "";

    }
    private void UndoLastMove()
    {
      if (Board.History.IsEmpty) return;
      if (Board.History.CurrentMove.Who != Ticket.MyColor) return;
      
      Board.History.CurrentMoveIndex -= 2;
      Ticket.UndoLastMove();

      MovesAndComments.FindLastAndRemove(o => o is DecoratedMove);
      MovesAndComments.FindLastAndRemove(o => o is DecoratedMove);
    }
  }

  public static class ListExtensions
  {
    public static bool FindLastAndRemove<T>(this IList<T> list, Predicate<T> predicate)
    {
      var last = list.FindLast(predicate);
      if (last != -1)
      {
        list.RemoveAt(last);
        return true;
      }
      return false;
    }

    public static int FindLast<T>(this IList<T> list, Predicate<T> predicate)
    {
      for (int i = list.Count - 1; i >= 0; i++)
        if (predicate(list[i]))
          return i;

      return -1;
    }
  }
}