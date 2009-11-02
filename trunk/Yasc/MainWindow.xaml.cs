using System;
using System.Windows;
using Yasc.Gui;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

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
//      _board.Move += BoardOnMove;
      Shogi.InititBoard(_board);
      DataContext = _board;
      if (!Server.ServerIsStartedOnThisComputer)
        StartServer();
    }
    private void OnMoveAttempt(object sender, MoveAttemptEventArgs e)
    {
      _errorLabel.Text = e.Move.ErrorMessage;
    }

//    private void BoardOnMove(object sender, MoveEventArgs args)
//    {
//      var m = args.Move as UsualMove;
//      if (m == null) return;
//      var from = _board[m.From.X, m.From.Y];
//      var to = _board[m.To.X, m.To.Y];
//      DependencyObject fromCtrl = _cells.ItemContainerGenerator.ContainerFromItem(from);
//      DependencyObject toCtrl = _cells.ItemContainerGenerator.ContainerFromItem(to);
//      MessageBox.Show(fromCtrl.ToString() + toCtrl);
//    }

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
 
    private void TicketOnOpponentMadeMove(string move)
    {
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

  }
}
