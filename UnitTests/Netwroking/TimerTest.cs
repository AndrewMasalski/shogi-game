using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Netwroking;
using Yasc.Networking;

namespace UnitTests
{
  [TestClass, NUnit.Framework.TestFixture]
  public class TimerTest
  {
    [TestMethod, NUnit.Framework.Test]
    public void Test1()
    {
      IPlayerGameController player1;
      IPlayerGameController player2;
      ServerRoutines.CrateGame(out player1, out player2);
      
      var localTimeBase = new []
                            {
                              new DateTime(2000, 10, 24),
                              new DateTime(2009, 08, 10)
                            };

      var progress = new Action<double>(seconds =>
        {
          localTimeBase[0] = localTimeBase[0].AddSeconds(seconds);
          localTimeBase[1] = localTimeBase[1].AddSeconds(seconds);
        });

      player1.OpponentMadeMove = msg =>
        {
          progress(.7);
          return localTimeBase[0];
        };
      player2.OpponentMadeMove = msg =>
        {
          progress(.1);
          return localTimeBase[1];
        };

      Assert.AreEqual(TimeSpan.FromMinutes(5), player1.TimeLeft);
      Assert.AreEqual(TimeSpan.FromMinutes(5), player2.TimeLeft);

      player1.Move(new MoveMsg("some move", localTimeBase[0]));

      Assert.AreEqual(TimeSpan.FromMinutes(5), player1.TimeLeft);
      Assert.AreEqual(TimeSpan.FromMinutes(5), player2.TimeLeft);

      progress(5);

      Assert.AreEqual(TimeSpan.FromMinutes(5), player1.TimeLeft);
      Assert.AreEqual(TimeSpan.FromMinutes(5), player2.TimeLeft);

      player2.Move(new MoveMsg("some move", localTimeBase[1]));

      Assert.AreEqual(TimeSpan.FromMinutes(5), player1.TimeLeft);
      Assert.AreEqual(TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(5), player2.TimeLeft);

      progress(3);
      player1.Move(new MoveMsg("some move", localTimeBase[0]));

      Assert.AreEqual(TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(3), player1.TimeLeft);
      Assert.AreEqual(TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(5), player2.TimeLeft);

      progress(7);
      player2.Move(new MoveMsg("some move", localTimeBase[1]));

      Assert.AreEqual(TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(3), player1.TimeLeft);
      Assert.AreEqual(TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(12), player2.TimeLeft);
    }
  }
}