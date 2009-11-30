using System;
using System.Collections.Generic;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
  /// <summary>Identifies shogi board cell coordinates in a user-friendly manner</summary>
  [Serializable]
  public struct Position
  {
    public int X { get; private set; }
    public int Y { get; private set; }
    public static IEnumerable<Position> OnBoard
    {
      get
      {
        for (int i = 0; i < 9; i++)
          for (int j = 8; j >= 0; j--)
            yield return new Position(j, i);
      }
    }
    public Position(string position)
      : this()
    {
      if (position == null) throw new ArgumentNullException("position");
      if (position.Length != 2) throw new ArgumentOutOfRangeException("position");
      X = int.Parse(position[0].ToString()) - 1;
      Y = char.ToLower(position[1]) - 'a';
      if (X < 0 || X > 8) throw new ArgumentOutOfRangeException("position");
      if (Y < 0 || Y > 8) throw new ArgumentOutOfRangeException("position");
    }

    public Position(int x, int y)
      : this()
    {
      X = x;
      Y = y;
      if (X < 0 || X > 8) throw new ArgumentOutOfRangeException("x");
      if (Y < 0 || Y > 8) throw new ArgumentOutOfRangeException("y");
    }
    public string Line
    {
      get { return ((char)(Y + 'a')).ToString(); }
    }
    public int Column
    {
      get { return X + 1; }
    }
    public override string ToString()
    {
      return Column + Line;
    }

    public static implicit operator Position(string s)
    {
      return new Position(s);
    }

    public bool Equals(Position other)
    {
      return other.X == X && other.Y == Y;
    }
    public override bool Equals(object obj)
    {
      if (obj.GetType() != typeof(Position)) return false;
      return Equals((Position)obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        return (X * 397) ^ Y;
      }
    }

    public static bool operator ==(Position left, Position right)
    {
      return left.Equals(right);
    }
    public static bool operator !=(Position left, Position right)
    {
      return !left.Equals(right);
    }
    public static Vector operator -(Position a, Position b)
    {
      return new Vector(b.X - a.X, b.Y - a.Y);
    }
  }
}