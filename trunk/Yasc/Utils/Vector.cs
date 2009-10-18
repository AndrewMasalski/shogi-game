using Yasc.ShogiCore.Utils;

namespace Yasc.Utils
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
    public static Position operator +(Position a, Vector b)
    {
      return new Position(a.X + b.X, a.Y + b.Y);
    }
    public static Vector operator *(Vector a, Vector b)
    {
      return new Vector(a.X*b.X, a.Y*b.Y);
    }
  }
}