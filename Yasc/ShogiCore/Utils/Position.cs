using System;
using System.Collections.Generic;
using Yasc.Utils;

namespace Yasc.ShogiCore.Utils
{
  /// <summary>Identifies shogi board cell coordinates in a user-friendly manner</summary>
  [Serializable]
  public struct Position
  {
    public int X { get; set; }
    public int Y { get; set; }
    public static IEnumerable<Position> OnBoard
    {
      get
      {
        for (int i = 0; i < 9; i++)
          for (int j = 0; j < 9; j++)
            yield return new Position(i, j);
      }
    }
    public Position(string position)
      : this()
    {
      if (position.Length != 2) throw new ArgumentOutOfRangeException("position");
      Y = char.ToLower(position[0]) - 'a';
      X = int.Parse(position[1].ToString()) - 1;
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
      return Line + Column;
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
      if (ReferenceEquals(null, obj)) return false;
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