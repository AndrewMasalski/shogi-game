using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShogiCore.UnitTests.Properties;
using Yasc.ShogiCore.Persistence;
using Yasc.ShogiCore.PieceSets;

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
        var board = trascription.LoadBoard(new StandardPieceSet());
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
          var board = trascription.LoadBoard(new StandardPieceSet());
          Assert.AreEqual(trascription.Moves.Count, board.History.Count);
        }
      }
    }
    [TestMethod]
    public void TranscriptionsCountTest()
    {
      CountMoves(36, Resources.Jan_Jun1992);
      CountMoves(4, Resources._16th_Kio_Match);
      CountMoves(7, Resources._20th_Osho_Match);
      CountMoves(6, Resources._26th_Osho_Match);
      CountMoves(1, Resources._29th_Osho_Match);
      CountMoves(7, Resources._31st_Meijin_Match);
      CountMoves(7, Resources._31st_Oi_Match);
      CountMoves(6, Resources._32nd_Oi_Match);
      CountMoves(4, Resources._32nd_Osho_Match);
      CountMoves(7, Resources._33rd_Meijin_Match);
      CountMoves(4, Resources._38th_Oza_Match);
      CountMoves(6, Resources._39th_Oza_Match);
      CountMoves(5, Resources._3rd_RyuO_Match);
      CountMoves(6, Resources._40th_Osho_Match);
      CountMoves(6, Resources._48th_Meijin_Match);
      CountMoves(5, Resources._49th_Meijin_Match);
      CountMoves(6, Resources._4th_RyuO_Match);
      CountMoves(5, Resources._56th_Kisei_Match);
      CountMoves(4, Resources._57th_Kisei_Match);
      CountMoves(5, Resources._58th_Kisei_Match);
      CountMoves(3, Resources._59th_Kisei_Match);
      CountMoves(28, Resources.misc_pro_games);
    }

    private static void CountMoves(int expectedMovesCount, byte[] file)
    {
      using (var s = new MemoryStream(file))
      {
        var trascriptions = new PsnTranscriber().Load(new StreamReader(s)).ToList();
        Assert.AreEqual(expectedMovesCount, trascriptions.Count);
        foreach (var gameTranscription in trascriptions)
        {
          gameTranscription.ParseBody();
          TrascriptionProperty cout;
          if (gameTranscription.Properties.TryGetValue("Moves", out cout))
          {
            Assert.AreEqual(int.Parse(cout.Value), gameTranscription.Moves.Count);
          }
          foreach (var moveTranscription in gameTranscription.Moves)
          {
            Assert.IsFalse(string.IsNullOrWhiteSpace(moveTranscription.MoveNotation));
          }
          gameTranscription.LoadSnapshot();
        }
      }
    }

    [TestMethod]
    public void TranscriptionContentTest()
    {
      using (var s = new MemoryStream(Resources.Jan_Jun1992))
      {
        var trascription = new PsnTranscriber().Load(new StreamReader(s)).First();
        Assert.AreEqual(5, trascription.Properties.Count);
        Assert.AreEqual("1992/01/16", trascription.Properties["Date"].Value);
        Assert.AreEqual("Osho-sen", trascription.Properties["Event"].Value);
        Assert.AreEqual("Shikenbisha", trascription.Properties["Opening"].Value);
        Assert.AreEqual("Minami Yoshikazu", trascription.Properties["Black"].Value);
        Assert.AreEqual("Tanigawa Koji", trascription.Properties["White"].Value);
      }
    }
    [TestMethod]
    public void TestBodyComment()
    {
      var gameTranscription = new GameTranscription();
      gameTranscription.Body.Append("{comment}");
      gameTranscription.ParseBody();
      Assert.AreEqual("comment", gameTranscription.GameComment);
    }
    [TestMethod]
    public void TestMoveComment()
    {
      var gameTranscription = new GameTranscription();
      gameTranscription.Body.Append("move {comment}");
      gameTranscription.ParseBody();
      Assert.AreEqual(1, gameTranscription.Moves.Count);
      var move = gameTranscription.Moves[0];
      Assert.AreEqual("move", move.MoveNotation);
      Assert.AreEqual("comment", move.Comment);
    }
    [TestMethod]
    public void TestMoveCommentNoSpace()
    {
      var gameTranscription = new GameTranscription();
      gameTranscription.Body.Append("move{comment}");
      gameTranscription.ParseBody();
      Assert.AreEqual(1, gameTranscription.Moves.Count);
      var move = gameTranscription.Moves[0];
      Assert.AreEqual("move", move.MoveNotation);
      Assert.AreEqual("comment", move.Comment);
    }
    [TestMethod]
    public void JustMove()
    {
      var gameTranscription = new GameTranscription();
      gameTranscription.Body.Append("move");
      gameTranscription.ParseBody();
      Assert.AreEqual(1, gameTranscription.Moves.Count);
      var move = gameTranscription.Moves[0];
      Assert.AreEqual("move", move.MoveNotation);
      Assert.AreEqual(null, move.Comment);
    }
  }
}