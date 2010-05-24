using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Lightweight store for piece</summary>
  [Serializable]
  public class PieceSnapshot
  {
    /// <summary>The piece type</summary>
    public IPieceType PieceType { get; private set; }
    /// <summary>The piece color</summary>
    public PieceColor Color { get; private set; }

    /// <summary>ctor</summary>
    public PieceSnapshot(IPieceType type, PieceColor color)
    {
      PieceType = type;
      Color = color;
    }

    /// <summary>Makes a promoted clon of this piece</summary>
    public PieceSnapshot ClonePromoted()
    {
      return new PieceSnapshot(PieceType.Promote(), Color);
    }
    /// <summary>Returns distance for from the last line for the piece (of the color)</summary>
    public int HowFarFromTheLastLine(Position position)
    {
      var lastLineIndex = Color == PieceColor.White ? 8 : 0;
      return Math.Abs(position.Y - lastLineIndex);
    }
    /// <summary>Returns null it the piece can move to the 
    ///   <paramref name="position"/> without promotion -or- 
    ///   text with explanation why he's not allowed to do that</summary>
    public string IsPromotionMandatory(Position position)
    {
      if (PieceType == PT.歩 || PieceType == PT.香)
        if (HowFarFromTheLastLine(position) == 0)
          return PieceType + " cannot move to "
                 + "the last line without promotion";

      if (PieceType == PT.桂)
        if (HowFarFromTheLastLine(position) < 2)
          return "桂 cannot move to the last two lines without promotion";

      return null;
    }
    /// <summary>Returns null it the piece can promote moving
    ///   from position <paramref name="from"/> to position <paramref name="to"/> -or- 
    ///   text with explanation why it's impossible</summary>
    public string IsPromotionAllowed(Position from, Position to)
    {
      if (PieceType.IsPromoted)
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
    /// <summary>Determines whether the specified PieceSnapshot is equal to the current PieceSnapshot</summary>
    public bool Equals(PieceSnapshot other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.PieceType, PieceType) && Equals(other.Color, Color);
    }
    /// <summary>Determines whether the specified PieceSnapshot is equal to the current PieceSnapshot</summary>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == typeof (PieceSnapshot) && Equals((PieceSnapshot) obj);
    }
    /// <summary>Serves as a hash function for a PieceSnapshot</summary>
    public override int GetHashCode()
    {
      unchecked
      {
        var result = PieceType.GetHashCode();
        result = (result*397) ^ Color.GetHashCode();
        return result;
      }
    }
    /// <summary>Determines whether two pieces are equal</summary>
    public static bool operator ==(PieceSnapshot left, PieceSnapshot right)
    {
      return Equals(left, right);
    }
    /// <summary>Determines whether two pieces are not equal</summary>
    public static bool operator !=(PieceSnapshot left, PieceSnapshot right)
    {
      return !Equals(left, right);
    }

    #endregion
    
    private bool IsThatPromitionZoneFor(Position position)
    {
      return HowFarFromTheLastLine(position) < 3;
    }

    /// <summary>Returns a <see cref="string"/> which represents the piece instance.</summary>
    public override string ToString()
    {
      return Color + " " + PieceType;
    }
  }
}