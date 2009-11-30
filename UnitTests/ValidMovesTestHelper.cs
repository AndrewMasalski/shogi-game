using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests
{
  public class ValidMovesTestHelper
  {
    private Position _startPosition;
    private Piece _piece;
    public Board Board { get; private set; }

    public ValidMovesTestHelper()
    {
      Board = new Board();
    }
    public void Init(Position startPosition, Piece piece)
    {
      _startPosition = startPosition;
      _piece = piece;
      Board[_startPosition] = _piece;
      Board.OneWhoMoves = _piece.Owner;
    }
    public void TestMoves(HashSet<Position> validMoves)
    {
      // Check valid moves
      foreach (var validCell in validMoves)
      {
        var move = Board.GetUsualMove(_startPosition, validCell, false);
        Assert.IsTrue(move.IsValid, move + ": " + move.ErrorMessage);
      }

      // Check invalid moves
      for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
          var cell = new Position(i, j);
          if (!validMoves.Contains(cell))
            Assert.IsFalse(Board.GetUsualMove(_startPosition, cell, false).IsValid);
        }
    }
  }
}