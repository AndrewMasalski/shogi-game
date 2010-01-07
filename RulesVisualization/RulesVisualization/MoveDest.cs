using Yasc.ShogiCore;

namespace Yasc.RulesVisualization
{
  public class MoveDest
  {
    public Position Position { get; set; }
    public bool Promotion { get; set; }

    public MoveDest(string str)
    {
      if (str.Length == 2)
      {
        Position = str;
      }
      else
      {
        Position = str.Substring(0, 2);
        Promotion = str.Substring(2, 1) == "+";
      }
    }
    public MoveDest(Position position, bool promotion)
    {
      Position = position;
      Promotion = promotion;
    }

    public bool Equals(MoveDest other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return other.Position.Equals(Position) && other.Promotion.Equals(Promotion);
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (MoveDest)) return false;
      return Equals((MoveDest) obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        return (Position.GetHashCode()*397) ^ Promotion.GetHashCode();
      }
    }
    public static bool operator ==(MoveDest left, MoveDest right)
    {
      return Equals(left, right);
    }
    public static bool operator !=(MoveDest left, MoveDest right)
    {
      return !Equals(left, right);
    }
  }
}