using System;
using System.Windows;
using Yasc.GenericDragDrop;
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
      Shogi.InititBoard(_board);
      DataContext = _board;
      if (!Server.ServerIsStartedOnThisComputer)
        StartServer();
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

    private void DropHandler(object sender, DropEventArgs e)
    {
      var from = (Cell) e.DragSource.DataContext;
      var to = (Cell) e.DragTarget.DataContext;
      _board.MakeMove(_board.GetUsualMove(from.Position, to.Position, false));
    }
  }
}
