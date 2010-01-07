using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Properties;
using Yasc.Persistence;
using System.Linq;

namespace UnitTests.Persistence
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
        Assert.AreEqual("Minami Yoshikazu", board.Black.Name);
        Assert.AreEqual("Tanigawa Koji", board.White.Name);
        Assert.AreEqual(trascription.Moves.Count, board.History.Count);
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
          Assert.AreEqual(trascription.Moves.Count, board.History.Count);
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
        Assert.AreEqual(6, trascription.Attributes.Count);
        Assert.AreEqual("1992/01/16", trascription.Attributes["Date"].Value);
        Assert.AreEqual("Osho-sen", trascription.Attributes["Event"].Value);
        Assert.AreEqual("Shikenbisha", trascription.Attributes["Opening"].Value);
        Assert.AreEqual("Minami Yoshikazu", trascription.Attributes["Black"].Value);
        Assert.AreEqual("Tanigawa Koji", trascription.Attributes["White"].Value);
        Assert.AreEqual("1-0", trascription.Attributes["_Footer"].Value);
      }      
    }
  }
}