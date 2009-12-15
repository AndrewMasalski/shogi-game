using System;

namespace Yasc.ShogiCore.Snapshots
{
  [Serializable]
  public class PieceSnapshot
  {
    public bool IsPromoted
    {
      get { return PieceType.IsPromoted; }
    }
    public PieceType PieceType { get; private set; }
    public PieceColor Color { get; private set; }

    public PieceSnapshot(Piece piece)
    {
      Color = piece.Color;
      PieceType = piece.PieceType;
    }
    public PieceSnapshot(PieceType type, PieceColor color)
    {
      PieceType = type;
      Color = color;
    }

    public PieceSnapshot ClonePromoted()
    {
      return new PieceSnapshot(PieceType.Promote(), Color);
    }
    public int HowFarFromTheLastLine(Position position)
    {
      var lastLineIndex = Color == PieceColor.White ? 8 : 0;
      return Math.Abs(position.Y - lastLineIndex);
    }
    public string IsPromotionMandatory(Position to)
    {
      if (PieceType == PieceType.歩 || PieceType == PieceType.香)
        if (HowFarFromTheLastLine(to) == 0)
          return PieceType + " cannot move to "
                 + "the last line without promotion";

      if (PieceType == PieceType.桂)
        if (HowFarFromTheLastLine(to) < 2)
          return "桂 cannot move to the last two lines without promotion";

      return null;
    }
    public string IsPromotionAllowed(Position from, Position to)
    {
      if (IsPromoted)
        return "Can't promote piece that is already promoted";

      if (!PieceType.CanPromote)
        return "Can't promote " + PieceType;

      if (!IsThatPromitionZoneFor(from))
        if (!IsThatPromitionZoneFor(to))
          return "Can't promote " + PieceType +
                 " when move from line " + from.Line + " to line " + to.Line;

      return null;
    }
    
    #region ' Equality '

    public bool Equals(PieceSnapshot other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.PieceType, PieceType) && Equals(other.Color, Color);
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
        int result = PieceType.GetHashCode();
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
    
    private bool IsThatPromitionZoneFor(Position position)
    {
      return HowFarFromTheLastLine(position) < 3;
    }
  }
}