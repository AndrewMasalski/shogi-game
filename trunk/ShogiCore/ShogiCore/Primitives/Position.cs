using System;
using System.Collections.Generic;
using Yasc.Utils;

namespace Yasc.ShogiCore.Primitives
{
  /// <summary>Identifies shogi board cell coordinates in a user-friendly manner</summary>
  [Serializable]
  public struct Position
  {
    /// <summary>X coordinate of the position</summary>
    public int X { get; private set; }
    /// <summary>Y coordinate of the position</summary>
    public int Y { get; private set; }
    

    /// <param name="position">"1a"</param>
    public static Position Parse(string position)
    {
      if (position == null) throw new ArgumentNullException("position");
      if (position.Length != 2) throw new ArgumentOutOfRangeException("position");
      var x = int.Parse(position[0].ToString()) - 1;
      var y = char.ToLower(position[1]) - 'a';
      if (x < 0 || x > 8) throw new ArgumentOutOfRangeException("position");
      if (y < 0 || y > 8) throw new ArgumentOutOfRangeException("position");
      return new Position { X = x, Y = y };
    }

    /// <summary>ctor</summary>
    public Position(int x, int y)
      : this()
    {
      X = x;
      Y = y;
      if (X < 0 || X > 8) throw new ArgumentOutOfRangeException("x");
      if (Y < 0 || Y > 8) throw new ArgumentOutOfRangeException("y");
    }

    /// <summary>The position line ("a".."i")</summary>
    public string Line
    {
      get { return ((char)(Y + 'a')).ToString(); }
    }
    /// <summary>The position column (1..9)</summary>
    public int Column
    {
      get { return X + 1; }
    }
    /// <summary>Gets user friendly transcription of the position ("1a")</summary>
    public override string ToString()
    {
      return Column + Line;
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

    #region ' Operators & Statics '

    /// <summary>Termwise subtraction</summary>
    public static Vector operator -(Position first, Position second)
    {
      return new Vector(second.X - first.X, second.Y - first.Y);
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

    /// <summary>Gets all 81 position on board</summary>
    public static IEnumerable<Position> OnBoard
    {
      get
      {
        for (var i = 0; i < 9; i++)
          for (var j = 8; j >= 0; j--)
            yield return new Position(j, i);
      }
    }

    #endregion
  }
}