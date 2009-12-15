using System;

namespace Yasc.ShogiCore.Snapshots
{
  [Serializable]
  public class PieceSnapshot
  {
    public bool IsPromoted
    {
      get { return Type.IsPromoted; }
    }
    public PieceType Type { get; private set; }
    public PieceColor Color { get; private set; }

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

    public PieceSnapshot ClonePromoted()
    {
      return new PieceSnapshot(Type.Promote(), Color);
    }
    public int HowFarFromTheLastLine(Position p)
    {
      var lastLineIndex = Color == PieceColor.White ? 8 : 0;
      return Math.Abs(p.Y - lastLineIndex);
    }
    public string IsPromotionMandatory(Position to)
    {
      if (Type == PieceType.歩 || Type == PieceType.香)
        if (HowFarFromTheLastLine(to) == 0)
          return Type + " cannot move to "
                 + "the last line without promotion";

      if (Type == PieceType.桂)
        if (HowFarFromTheLastLine(to) < 2)
          return "桂 cannot move to the last two lines without promotion";

      return null;
    }
    public string IsPromotionAllowed(Position from, Position to)
    {
      if (IsPromoted)
        return "Can't promote piece that is already promoted";

      if (!Type.CanPromote)
        return "Can't promote " + Type;

      if (!IsThatPromitionZoneFor(from))
        if (!IsThatPromitionZoneFor(to))
          return "Can't promote " + Type +
                 " when move from line " + from.Line + " to line " + to.Line;

      return null;
    }
    
    #region ' Equality '

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

    #endregion
    
    private bool IsThatPromitionZoneFor(Position p)
    {
      return HowFarFromTheLastLine(p) < 3;
    }
  }
}