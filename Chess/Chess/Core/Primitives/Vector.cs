namespace Chess
{
  public struct Vector
  {
    public int X { get; private set; }
    public int Y { get; private set; }

    public Vector(int x, int y)
      : this()
    {
      X = x;
      Y = y;
    }

    public static Vector operator +(Vector first, Vector second)
    {
      return new Vector(first.X + second.X, first.Y + second.Y);
    }
    public static Vector operator *(Vector first, Vector second)
    {
      return new Vector(first.X * second.X, first.Y * second.Y);
    }

    public bool Equals(Vector other)
    {
      return other.X == X && other.Y == Y;
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (obj.GetType() != typeof(Vector)) return false;
      return Equals((Vector)obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        return (X * 397) ^ Y;
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

    public override string ToString()
    {
      return string.Format("({0},{1})", X, Y);
    }
  }
}