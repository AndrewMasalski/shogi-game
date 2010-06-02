using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShogiCore.UnitTests.Properties;
using Yasc.ShogiCore.Persistence;

namespace ShogiCore.UnitTests.Persistence
{
  [TestClass]
  public class PsnLoaderTest
  {
    [TestMethod]
    public void LoadTranscription()
    {
      using (var s = new MemoryStream(Resources.Jan_Jun1992))
      {
        var trascription = new PsnTranscriber().Load(new StreamReader(s)).First();
        var board = new PsnLoader().Load(trascription);
//        Assert.AreEqual("Minami Yoshikazu", board.Black.Name);
//        Assert.AreEqual("Tanigawa Koji", board.White.Name);
        Assert.AreEqual(trascription.Moves.Count, board.Move.Number);
      }    
    }
    [TestMethod]
    public void CheckWeCanLoadThemAll()
    {
      using (var s = new MemoryStream(Resources.Jan_Jun1992))
      {
        foreach (var trascription in new PsnTranscriber().Load(new StreamReader(s)))
        {
          var board = new PsnLoader().Load(trascription);
          Assert.AreEqual(trascription.Moves.Count, board.Move.Number);
        }
      }    
    }
    [TestMethod]
    public void TranscriptionsCountTest()
    {
      using (var s = new MemoryStream(Resources.Jan_Jun1992))
      {
        var trascriptions = new PsnTranscriber().Load(new StreamReader(s)).ToList();
        Assert.AreEqual(35, trascriptions.Count);
      }
    }
    [TestMethod]
    public void TranscriptionContentTest()
    {
      using (var s = new MemoryStream(Resources.Jan_Jun1992))
      {
        var trascription = new PsnTranscriber().Load(new StreamReader(s)).First();
        Assert.AreEqual(6, trascription.Properties.Count);
        Assert.AreEqual("1992/01/16", trascription.Properties["Date"].Value);
        Assert.AreEqual("Osho-sen", trascription.Properties["Event"].Value);
        Assert.AreEqual("Shikenbisha", trascription.Properties["Opening"].Value);
        Assert.AreEqual("Minami Yoshikazu", trascription.Properties["Black"].Value);
        Assert.AreEqual("Tanigawa Koji", trascription.Properties["White"].Value);
        Assert.AreEqual("1-0", trascription.Properties["_Footer"].Value);
      }      
    }
  }
}