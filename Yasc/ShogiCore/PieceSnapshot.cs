using System;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore
{
  [Serializable]
  public class PieceSnapshot
  {
    public PieceType Type { get; private set; }
    public PieceColor Color { get; private set; }

    public bool IsPromoted
    {
      get { return Type.IsPromoted; }
    }

    public PieceSnapshot(Piece piece)
    {
      Color = piece.Color;
      Type = piece.Type;
    }
    public PieceSnapshot(PieceType type, PieceColor color)
    {
      Type = type;
      Color = color;
    }

    public bool IsThatPromitionZoneFor(Position p)
    {
      return HowFarFromTheLastLine(p) < 3;
    }
    public int HowFarFromTheLastLine(Position p)
    {
      var lastLineIndex = Color == PieceColor.White ? 8 : 0;
      return Math.Abs(p.Y - lastLineIndex);
    }

    public bool Equals(PieceSnapshot other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.Type, Type) && Equals(other.Color, Color);
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (PieceSnapshot)) return false;
      return Equals((PieceSnapshot) obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        int result = Type.GetHashCode();
        result = (result*397) ^ Color.GetHashCode();
        return result;
      }
    }
    public static bool operator ==(PieceSnapshot left, PieceSnapshot right)
    {
      return Equals(left, right);
    }
    public static bool operator !=(PieceSnapshot left, PieceSnapshot right)
    {
      return !Equals(left, right);
    }

    public PieceSnapshot ClonePromoted()
    {
      return new PieceSnapshot(Type.Promote(), Color);
    }
  }
}