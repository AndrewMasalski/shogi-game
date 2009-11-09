using System;
using System.Windows;
using System.Windows.Input;
using Yasc.Gui;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;
using Yasc.GenericDragDrop;
using Yasc.Utils;

namespace Yasc
{
  public partial class MainWindow 
  {
    private readonly Board _board;
    private IPlayerGameController _ticket;

    public MainWindow()
    {
      InitializeComponent();

      _board = new Board();
      _board.Move += BoardOnMove;
      Shogi.InititBoard(_board);
      DataContext = _board;
      if (!Server.ServerIsStartedOnThisComputer)
        StartServer();
    }

    private void BoardOnMove(object sender, MoveEventArgs args)
    {
      if (!_opponentMoveReaction && _ticket != null)
        _ticket.Move(args.Move.ToString());
    }

    private void OnMoveAttempt(object sender, MoveAttemptEventArgs e)
    {
      _errorLabel.Text = e.Move.ErrorMessage;
    }

    private void OnConnectClick(object sender, RoutedEventArgs e)
    {
      try
      {
        var server = Server.Connect(_address.Text);
        _ticket = server.ParticipateGame();
        if (_ticket == null) throw new Exception("There might be two players only");
        _ticket.OpponentMadeMove += new ActionListener<string>(TicketOnOpponentMadeMove);
        _connectButton.Content = "Done!";
      }
      catch (Exception x)
      {
        MessageBox.Show(x.ToString());
      }
    }

    private readonly Flag _opponentMoveReaction = new Flag();

    private void TicketOnOpponentMadeMove(string move)
    {
      using (_opponentMoveReaction.Set())
        _board.MakeMove(_board.GetMove(move));
    }
    private void OnMoveClick(object sender, RoutedEventArgs e)
    {
      try
      {
        var move = _moveTextBox.Text;
        _board.MakeMove(_board.GetMove(move));
        if (_ticket != null) _ticket.Move(move);
      }
      catch (Exception x)
      {
        MessageBox.Show(x.ToString());
      }
    }
    private void OnStartServer(object sender, RoutedEventArgs e)
    {
      StartServer();
    }

    private void StartServer()
    {
      Server.Start();
      _startServerButton.Content = "Done!";
    }

    private void MouseMove1(object sender, MouseEventArgs e)
    {
      var obj = e.OriginalSource as DependencyObject;
      if (obj != null)
      {
        var ancestor = obj.FindAncestor<ShogiPiece>();
        if (ancestor != null) Title = ancestor.ToString();
      }
    }
  }
}
