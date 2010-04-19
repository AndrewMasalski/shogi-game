﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using MvvmFoundation.Wpf;
using Yasc.Persistence;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using System.Linq;

namespace Yasc.Gui.Game
{
  public abstract class GameViewModel : ObservableObject
  {
    #region ' Fields '

    private string _currentMessage;
    private TimeSpan _opponentTime = TimeSpan.FromSeconds(300);
    private TimeSpan _ourTime = TimeSpan.FromSeconds(300);
    private bool _isFlipped;
    private RelayCommand _cleanBoardCommand;
    private RelayCommand _getBackCommand;
    private bool _isItOpponentMove;
    private bool _isItMyMove;
    private bool _isMyTimerLaunched;
    private bool _isOpponentTimerLaunched;
    private RelayCommand _loadTranscriptCommand;
    private Board _board;

    #endregion

    #region ' Public Interface '

    public string CurrentMessage
    {
      get { return _currentMessage; }
      set
      {
        if (_currentMessage == value) return;
        _currentMessage = value;
        RaisePropertyChanged("CurrentMessage");
      }
    }
    public bool IsItMyMove
    {
      get { return _isItMyMove; }
      set
      {
        if (_isItMyMove == value) return;
        _isItMyMove = value;
        RaisePropertyChanged("IsItMyMove");
        IsItOpponentMove = !value;
      }
    }
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
    public bool IsOpponentTimerLaunched
    {
      get { return _isOpponentTimerLaunched; }
      set
      {
        if (_isOpponentTimerLaunched == value) return;
        _isOpponentTimerLaunched = value;
        RaisePropertyChanged("IsOpponentTimerLaunched");
      }
    }
    public bool IsMyTimerLaunched
    {
      get { return _isMyTimerLaunched; }
      set
      {
        if (_isMyTimerLaunched == value) return;
        _isMyTimerLaunched = value;
        RaisePropertyChanged("IsMyTimerLaunched");
      }
    }

    public Board Board
    {
      get { return _board; }
      private set
      {
        if (_board == value) return;
        if (_board != null)
        {
          Board.Moved -= BoardOnMoved;
        }
        _board = value;
        if (_board != null)
        {
          Board.Moved += BoardOnMoved;
        }
        RaisePropertyChanged("Board");
      }
    }
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
        if (value == TimeSpan.Zero)
        {
          IsMyTimerLaunched = false;
          IsItMyMove = false;
        }
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
    public ObservableCollection<object> MovesAndComments { get; private set; }

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
    public ICommand LoadTranscriptCommand
    {
      get
      {
        if (_loadTranscriptCommand == null)
        {
          _loadTranscriptCommand = new RelayCommand(LoadTranscript);
        }
        return _loadTranscriptCommand;
      }
    }

    public event EventHandler GameOver;

    #endregion

    #region ' Implementation '

    protected void InitBoard()
    {
      Board = new Board();
      Shogi.InitBoard(Board);
      MovesAndComments = new ObservableCollection<object>();
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
    protected virtual void BoardOnMoved(object sender, MoveEventArgs args)
    {

      OnAnyMove();
    }
    private void OnAnyMove()
    {
      IsItMyMove = !IsItMyMove;
      IsMyTimerLaunched = IsItMyMove;
      IsOpponentTimerLaunched = IsItOpponentMove;
      MovesAndComments.Add(Board.History.CurrentMove);
    }

    protected static PieceColor Opponent(PieceColor color)
    {
      return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
    private void LoadTranscript()
    {
      var dlg = new OpenFileDialog
                  {
                    CheckFileExists = true,
                    DefaultExt = ".psn",
                    DereferenceLinks = true,
                    Title = "Choose PSN transcript file to open",
                    Filter = "PSN files|*.psn"
                  };
      if (dlg.ShowDialog() == true)
      {
        Board = new PsnLoader().Load(
          new PsnTranscriber().Load(File.OpenText(dlg.FileName)).
            First());
      }
    }

    #endregion
  }
}