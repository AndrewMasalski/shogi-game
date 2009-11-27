using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  public class MovesDescr
  {
    public List<MoveBase> Moves { get; private set; }
    public bool IsExclusive { get; set; }

    public MovesDescr(IUsualMoves moves)
    {
      Moves = new List<MoveBase>();
    }
  }

  [ContentProperty("Content")]
  public class ValidMovesManifest
  {
    public List<object> Content { get; set; }

    public ValidMovesManifest()
    {
      Content = new List<object>();
    }
  }
  public abstract class UsualMovesBase : IUsualMoves
  {
    public string From { get; set; }
    public string To { get; set; }
    public MovesValidatorMode Mode { get; set; }

    Position IUsualMoves.From
    {
      get { return From; }
    }
    IEnumerable<Position> IUsualMoves.To
    {
      get { return IsAvailable ? GetTo() : GetNotTo(); }
    }
    IEnumerable<Position> IUsualMoves.NotTo
    {
      get { return IsAvailable ? GetNotTo() : GetTo(); }
    }

    private IEnumerable<Position> GetTo()
    {
      return from s in To.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries) select (Position)s;
    }
    private IEnumerable<Position> GetNotTo()
    {
      switch (Mode)
      {
        case MovesValidatorMode.AndNoMore:
          var set = new HashSet<Position>(GetTo());
          return from p in Position.OnBoard where !set.Contains(p) select p;
        default:
          return new Position[0];
      }
    }

    public abstract bool IsAvailable { get; }
  }
  public class AvailableUsualMoves : UsualMovesBase
  {
    public override bool IsAvailable
    {
      get { return true; }
    }
  }

  public class ForbiddenUsualMoves : UsualMovesBase
  {
    public override bool IsAvailable
    {
      get { return false; }
    }
  }

  public interface IUsualMoves
  {
    Position From { get; }
    IEnumerable<Position> To { get; }
    IEnumerable<Position> NotTo { get; }
  }

  public interface IDropMoves
  {
    PieceColor For { get; }
    PieceType Piece { get; }
    string To { get; }
    MovesValidatorMode Mode { get; }
    bool IaAllowed { get; }
  }

  public class AvailableDropMoves : IDropMoves
  {
    public PieceColor For { get; set; }
    public PieceType Piece { get; set; }
    public string To { get; set; }
    public MovesValidatorMode Mode { get; set; }
    public bool IaAllowed
    {
      get { return true; }
    }
  }
  public class ForbiddenDropMoves : IDropMoves
  {
    public PieceColor For { get; set; }
    public PieceType Piece { get; set; }
    public string To { get; set; }
    public MovesValidatorMode Mode { get; set; }
    public bool IaAllowed
    {
      get { return false; }
    }
  }

  public enum MovesValidatorMode
  {
    AndOther, AndNoMore
  }
}