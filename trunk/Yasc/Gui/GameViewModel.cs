﻿using System;
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
    private readonly IServerGame _game;
    private RelayCommand _getBackCommand;
    private IPlayerGameController _ticket;
    private readonly Flag _opponentMoveReaction = new Flag();
    private bool _isFlipped;

    public Board Board { get; private set; }
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

    private void CleanBoard()
    {
      foreach (var cell in Board.Cells)
      {
        if (cell.Piece != null)
        {
          cell.Piece.Owner.Hand.Add(cell.Piece);
          cell.ResetPiece();
        }
      }
    }

    private RelayCommand _cleanBoardCommand;

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

    private void Init(IPlayerGameController ticket)
    {
      _ticket = ticket;
      _ticket.OpponentMadeMove = new FuncListener<MoveMsg, DateTime>(OnOpponentMadeMove);

      IsFlipped = _ticket.MyColor == PieceColor.White;
      InitBoard();
    }
    private void InitBoard()
    {
      Board = new Board();
      Board.Moved += BoardOnMoved;
      Shogi.InititBoard(Board);
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

    private void BoardOnMoved(object sender, MoveEventArgs args)
    {
      // If it's not opponent than it must be me
      if (!_opponentMoveReaction && _ticket != null)
      {
        _ticket.Move(new MoveMsg(args.Move.ToString()));
        if (Board.CurrentSnapshot.IsMateFor(Opponent(_ticket.MyColor)))
        {
          MessageBox.Show("You won!");
        }
      }
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
  }
}
