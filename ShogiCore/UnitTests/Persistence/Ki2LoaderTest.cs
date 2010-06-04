using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShogiCore.UnitTests.Properties;
using Yasc.ShogiCore.Persistence;
using Yasc.ShogiCore.PieceSets;

namespace ShogiCore.UnitTests.Persistence
{
  [TestClass]
  public class Ki2LoaderTest
  {
    [TestMethod]
    public void LoadTranscription()
    {
      using (var s = new StringReader(Resources._25_0102))
      {
        var trascription = new Ki2Transcriber().Load(s).First();
        var board = trascription.LoadBoard(new StandardPieceSet());
        Assert.AreEqual("Minami Yoshikazu", board.Black.Name);
        Assert.AreEqual("Tanigawa Koji", board.White.Name);
        Assert.AreEqual(113, board.History.Count);
        Assert.AreEqual(trascription.Moves.Count, board.History.Count);
      }
    }    
  }
}