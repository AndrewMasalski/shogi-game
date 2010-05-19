using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasc.ShogiCore.Primitives
{
  /// <summary>Represents piece type</summary>
  [Serializable]
  public struct PieceType : IComparable<PieceType>
  {
    #region ' Field '

    private enum RealType
    {
      Kr, Kc, R, B, G, S, N, L, P, Rp, Bp, Sp, Np, Lp, Pp
    }
    private readonly RealType _value;

    /// <summary>Gets all known piece types</summary>
    public static IEnumerable<PieceType> AllPieceTypes
    {
      get 
      { 
        return from RealType rt in Enum.GetValues(typeof (RealType)) 
               select new PieceType(rt); 
      }
    }

    #endregion

    /// <summary>Indicates wheter the piece type is promoted</summary>
    public bool IsPromoted
    {
      get
      {
        switch (_value)
        {
          case RealType.Kr: return false;
          case RealType.Kc: return false;
          case RealType.R: return false;
          case RealType.B: return false;
          case RealType.G: return false;
          case RealType.S: return false;
          case RealType.N: return false;
          case RealType.L: return false;
          case RealType.P: return false;
          case RealType.Rp: return true;
          case RealType.Bp: return true;
          case RealType.Sp: return true;
          case RealType.Np: return true;
          case RealType.Lp: return true;
          default:
          /*case RealType.Pp: */return true;
        }
      }
    }
    /// <summary>Indicates whether the piece type can be promoted</summary>
    public bool CanPromote
    {
      get
      {
        switch (_value)
        {
          case RealType.Kr: return false;
          case RealType.Kc: return false;
          case RealType.R: return true;
          case RealType.B: return true;
          case RealType.G: return false;
          case RealType.S: return true;
          case RealType.N: return true;
          case RealType.L: return true;
          case RealType.P: return true;
          case RealType.Rp: return false;
          case RealType.Bp: return false;
          case RealType.Sp: return false;
          case RealType.Np: return false;
          case RealType.Lp: return false;
            default:
          /*case RealType.Pp:*/ return false;
        }
      }
    }
    /// <summary>Gets the latin version of piece type</summary>
    public string Latin
    {
      get { return _value.ToString(); }
    }
    /// <summary>Gets the id of the piece type which is the same for 
    ///   promoted and unpromoted versions of the piece</summary>
    public int Id
    {
      get { return (int)(IsPromoted ? Demote() : this)._value; }
    }

    /// <summary>Returns promoted version of the piece type</summary>
    public PieceType Promote()
    {
      switch (_value)
      {
        case RealType.R: return new PieceType(RealType.Rp);
        case RealType.B: return new PieceType(RealType.Bp);
        case RealType.S: return new PieceType(RealType.Sp);
        case RealType.N: return new PieceType(RealType.Np);
        case RealType.L: return new PieceType(RealType.Lp);
        case RealType.P: return new PieceType(RealType.Pp);
      }
      throw new InvalidOperationException("Can't promote " + this);
    }
    /// <summary>Returns "unpromoted" version of the piece type</summary>
    public PieceType Demote()
    {
      switch (_value)
      {
        case RealType.Rp: return new PieceType(RealType.R);
        case RealType.Bp: return new PieceType(RealType.B);
        case RealType.Sp: return new PieceType(RealType.S);
        case RealType.Np: return new PieceType(RealType.N);
        case RealType.Lp: return new PieceType(RealType.L);
        case RealType.Pp: return new PieceType(RealType.P);
      }
      throw new InvalidOperationException("Can't demote " + this);
    }
    /// <summary>Get human readable representation of piece type</summary>
    public override string ToString()
    {
      return this;
    }

    #region ' Types '

    private PieceType(RealType type)
      : this()
    {
      _value = type;
    }

    /// <summary>King (reigning)</summary>
    public static readonly PieceType 王 = new PieceType(RealType.Kr);
    /// <summary>King (challenging)</summary>
    public static readonly PieceType 玉 = new PieceType(RealType.Kc);
    /// <summary>Rook</summary>
    public static readonly PieceType 飛 = new PieceType(RealType.R); //new PieceType("飛", "Хися", "竜");
    /// <summary>Bishop</summary>
    public static readonly PieceType 角 = new PieceType(RealType.B); // = new PieceType("角", "Какугё", "馬");
    /// <summary>Gold general</summary>
    public static readonly PieceType 金 = new PieceType(RealType.G); // = new PieceType("金", "Кин", null);
    /// <summary>Silver general</summary>
    public static readonly PieceType 銀 = new PieceType(RealType.S); // = new PieceType("銀", "Гин", "金");
    /// <summary>Knight</summary>
    public static readonly PieceType 桂 = new PieceType(RealType.N); // = new PieceType("桂", "Кэйма", "成");
    /// <summary>Lance</summary>
    public static readonly PieceType 香 = new PieceType(RealType.L); // = new PieceType("香", "Кёся", "成");
    /// <summary>Pawn</summary>
    public static readonly PieceType 歩 = new PieceType(RealType.P); // = new PieceType("歩", "Фухё", "と");
    /// <summary>Promoted Rook</summary>
    public static readonly PieceType 竜 = new PieceType(RealType.Rp);
    /// <summary>Promoted Bishop</summary>
    public static readonly PieceType 馬 = new PieceType(RealType.Bp);
    /// <summary>Promoted Silver</summary>
    public static readonly PieceType 全 = new PieceType(RealType.Sp);
    /// <summary>Promoted Pawn</summary>
    public static readonly PieceType と = new PieceType(RealType.Pp);
    /// <summary>Promoted Knight</summary>
    public static readonly PieceType 今 = new PieceType(RealType.Np);
    /// <summary>Promoted Lance</summary>
    public static readonly PieceType 仝 = new PieceType(RealType.Lp);

    /// <summary>Implicit type conversion operator. Uses japanese hieroglyphs</summary>
    public static implicit operator string(PieceType type)
    {
      switch (type._value)
      {
        case RealType.Kr: return "王";
        case RealType.Kc: return "玉";
        case RealType.R: return "飛";
        case RealType.B: return "角";
        case RealType.G: return "金";
        case RealType.S: return "銀";
        case RealType.N: return "桂";
        case RealType.L: return "香";
        case RealType.P: return "歩";
        case RealType.Rp: return "竜";
        case RealType.Bp: return "馬";
        case RealType.Sp: return "全";
        case RealType.Np: return "今";
        case RealType.Lp: return "仝";
        default: return "と";
      }
    }
    /// <summary>Implicit type conversion operator. 
    ///   Recognizes japanese hieroglyphs as well as latin symbols</summary>
    public static explicit operator PieceType(string symbol)
    {
      if (symbol == null) throw new ArgumentNullException("symbol");
      switch (symbol)
      {
        case "王": return 王;
        case "玉": return 玉;
        case "飛": return 飛;
        case "角": return 角;
        case "金": return 金;
        case "銀": return 銀;
        case "桂": return 桂;
        case "香": return 香;
        case "歩": return 歩;

        case "竜": return 竜;
        case "馬": return 馬;
        case "全": return 全;
        case "今": return 今;
        case "仝": return 仝;
        case "と": return と;

        case "Kr": return 王;
        case "Kc": return 玉;
        case "R": return 飛;
        case "B": return 角;
        case "G": return 金;
        case "S": return 銀;
        case "N": return 桂;
        case "L": return 香;
        case "P": return 歩;

        case "+R": return 竜;
        case "+B": return 馬;
        case "+S": return 全;
        case "+N": return 今;
        case "+L": return 仝;
        case "+P": return と;
      }
      throw new ArgumentOutOfRangeException("symbol");
    }

    #endregion

    #region ' Equality '

    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    public bool Equals(PieceType other)
    {
      return Equals(other._value, _value);
    }
    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj.GetType() == typeof (PieceType) && Equals((PieceType) obj);
    }
    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode()
    {
      return _value.GetHashCode();
    }
    /// <summary>Indicates whether two instances are equal.</summary>
    public static bool operator ==(PieceType left, PieceType right)
    {
      return left.Equals(right);
    }
    /// <summary>Indicates whether two instances are not equal.</summary>
    public static bool operator !=(PieceType left, PieceType right)
    {
      return !left.Equals(right);
    }
    /// <summary>Compares the current object with another object of the same type.</summary>
    public int CompareTo(PieceType other)
    {
      return (int)_value - (int)other._value;
    }
    /// <summary>Compares two types.</summary>
    public static bool operator >(PieceType left, PieceType right)
    {
      return left.CompareTo(right) > 0;
    }
    /// <summary>Compares two types.</summary>
    public static bool operator <(PieceType left, PieceType right)
    {
      return left.CompareTo(right) < 0;
    }

    #endregion

    /// <summary>Gets all valid IDs</summary>
    /// <seealso cref="Id"/>
    public static IEnumerable<int> AllValidIds
    {
      get
      {
        for (var i = 0; i < 9; i++)
          yield return i;
      }
    }
    /// <summary>Gets piece type by ID</summary>
    public static PieceType GetPieceType(int id)
    {
      return new PieceType((RealType) id);
    }
  }
}