using Yasc.ShogiCore;

namespace Yasc.Utils
{
  public struct Vector
  {
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool IsValidPosition
    {
      get { return X >= 0 && X < 9 && Y >= 0 && Y < 9; }
    }

    public Vector(int x, int y)
      : this()
    {
      X = x;
      Y = y;
    }

    public static Vector operator +(Vector vector, Position position)
    {
      return new Vector(position.X + vector.X, position.Y + vector.Y);
    }
    public static Vector operator +(Position position, Vector vector)
    {
      return new Vector(position.X + vector.X, position.Y + vector.Y);
    }
    public static Vector operator +(Vector first, Vector second)
    {
      return new Vector(first.X + second.X, first.Y + second.Y);
    }
    public static Vector operator *(Vector first, Vector second)
    {
      return new Vector(first.X*second.X, first.Y*second.Y);
    }
    public static implicit operator Vector(Position position)
    {
      return new Vector(position.X, position.Y);
    }
    public static implicit operator Position(Vector vector)
    {
      return new Position(vector.X, vector.Y);
    }

    public bool Equals(Vector other)
    {
      return other.X == X && other.Y == Y;
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (obj.GetType() != typeof (Vector)) return false;
      return Equals((Vector) obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        return (X*397) ^ Y;
      }
    }
    public static bool operator ==(Vector left, Vector right)
    {
      return left.Equals(right);
    }
    public static bool operator !=(Vector left, Vector right)
    {
      return !left.Equals(right);
    }
  }
}