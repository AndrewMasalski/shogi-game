using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class TimerTest
  {
    [TestMethod]
    public void Test1()
    {
      var board = new Board();
      Shogi.InititBoard(board);
      board.GetMove(new MoveMsg("3c-3d", 
        DateTime.Now.Add(TimeSpan.FromSeconds(0))));
      board.GetMove(new MoveMsg("7g-7f", 
        DateTime.Now.Add(TimeSpan.FromSeconds(20))));
    }
  }
}