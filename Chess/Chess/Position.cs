using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Converters;
using System.Windows.Markup;

namespace Chess
{
  /// <summary>Identifies chess board cell coordinates in a user-friendly manner</summary>
  /// <remarks>http://en.wikipedia.org/wiki/Chess</remarks>
  [TypeConverter(typeof(PositionConverter))]
  [ValueSerializer(typeof(PositionValueSerializer))]
  [Serializable]
  public struct Position
  {
    /// <summary>X coordinate</summary>
    public int X { get; private set; }
    /// <summary>Y coordinate</summary>
    public int Y { get; private set; }

    /// <param name="position">"a1" or "A1"</param>
    public static Position Parse(string position)
    {
      if (position == null) throw new ArgumentNullException("position");
      if (position.Length != 2) throw new ArgumentOutOfRangeException("position");
      var x = char.ToLower(position[0]) - 'a';
      var y = int.Parse(position[1].ToString()) - 1;
      if (x < 0 || x > 7) throw new ArgumentOutOfRangeException("position");
      if (y < 0 || y > 7) throw new ArgumentOutOfRangeException("position");
      return new Position(x, y);
    }

    /// <summary>ctor</summary>
    public Position(int x, int y)
      : this()
    {
      X = x;
      Y = y;
      if (X < 0 || X > 7) throw new ArgumentOutOfRangeException("x");
      if (Y < 0 || Y > 7) throw new ArgumentOutOfRangeException("y");
    }

    /// <summary>Column("a".."h")</summary>
    public string File
    {
      get { return ((char)(X + 'a')).ToString(); }
    }
    /// <summary>Row (1..8)</summary>
    public int Rank
    {
      get { return Y + 1; }
    }
    /// <summary>Gets user friendly transcription of the position ("a1")</summary>
    public override string ToString()
    {
      return File + Rank;
    }

    #region  ' Equality '

    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    public bool Equals(Position other)
    {
      return other.X == X && other.Y == Y;
    }
    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    public override bool Equals(object obj)
    {
      if (obj == null) return false;
      return obj.GetType() == typeof (Position) && Equals((Position) obj);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode()
    {
      unchecked
      {
        return (X * 397) ^ Y;
      }
    }
    /// <summary>Indicates whether two instances are equal.</summary>
    public static bool operator ==(Position left, Position right)
    {
      return left.Equals(right);
    }
    /// <summary>Indicates whether two instances are not equal.</summary>
    public static bool operator !=(Position left, Position right)
    {
      return !left.Equals(right);
    }

    #endregion

    #region ' Operators '

    /// <summary>Termwise subtraction</summary>
    public static Vector operator -(Position first, Position second)
    {
      return new Vector(first.X - second.X, first.Y - second.Y);
    }
    /// <summary>Termwise addtition</summary>
    public static Vector operator +(Vector vector, Position position)
    {
      return new Vector(position.X + vector.X, position.Y + vector.Y);
    }
    /// <summary>Termwise addtition</summary>
    public static Vector operator +(Position position, Vector vector)
    {
      return new Vector(position.X + vector.X, position.Y + vector.Y);
    }
    /// <summary>Position->Vector</summary>
    public static explicit operator Vector(Position position)
    {
      return new Vector(position.X, position.Y);
    }
    /// <summary>Vector->Position</summary>
    public static implicit operator Position(Vector vector)
    {
      return new Position(vector.X, vector.Y);
    }

    #endregion

    /// <summary>Gets all 64 position on board</summary>
    public static IEnumerable<Position> OnBoard
    {
      get
      {
        for (var i = 0; i < 8; i++)
          for (var j = 0; j < 8; j++)
            yield return new Position(j, i);
      }
    }
  }
}