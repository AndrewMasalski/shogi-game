using System;
using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace Yasc
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();

//      RemotingConfiguration.Configure("YetOneMoreChess.exe.config", false);
//      Board.Connected += (sender, args) =>
//        Dispatcher.BeginInvoke(new Action(ChessBoardOnConnected));
//      ChessBoardOnConnected();
      var board = new Board();
      Shogi.InititBoard(board);
      DataContext = board;
    }

//    private void ChessBoardOnConnected()
//    {
//      var battleField = new Board();
//      try
//      {
//        foreach (var p in Position.OnBoard)
//            _board.Children.Add(new ContentControl
//            {
//              Content = battleField[p.X, p.Y]
//            });
//      }
//      catch (Exception e)
//      {
//        MessageBox.Show(e.Message);
//      }
//    }

//    private void Button_Click(object sender, RoutedEventArgs e)
//    {
//      var server = (Board)Activator.GetObject(typeof(Board),
//          string.Format("tcp://{0}:1937/Server.rem", "ws211"));

//      var board = new Board();

//      for (int i = 0; i < 8; i++)
//        for (int j = 0; j < 8; j++)
//          _board.Children.Add(new ContentControl
//          {
//            Content = board[j, i]
//          });
//    }

  }
}
