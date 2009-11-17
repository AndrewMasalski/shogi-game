using System;
using Yasc.ShogiCore.Utils;

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
    public static Vector operator +(Position a, Vector b)
    {
      return new Vector(a.X + b.X, a.Y + b.Y);
    }
    public static Vector operator +(Vector a, Vector b)
    {
      return new Vector(a.X + b.X, a.Y + b.Y);
    }
    public static Vector operator *(Vector a, Vector b)
    {
      return new Vector(a.X*b.X, a.Y*b.Y);
    }
    public static implicit operator Vector(Position p)
    {
      return new Vector(p.X, p.Y);
    }
    public static implicit operator Position(Vector p)
    {
      return new Position(p.X, p.Y);
    }
  }
}