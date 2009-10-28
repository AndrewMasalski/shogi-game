using System;

namespace Yasc.ShogiCore
{
  [Serializable]
  public struct PieceType
  {
    private enum RealType
    {
      K, R, B, G, S, N, L, P, Rp, Bp, Sp, Np, Lp, Pp
    }
    public bool IsPromoted
    {
      get { return _value.ToString().Length == 2; }
    }

    public bool CanPromote
    {
      get
      {
        switch (_value)
        {
          case RealType.K: return false;
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
          case RealType.Pp: return false;
        }
        throw new Exception();
      }
    }

    public string Latin
    {
      get { return _value.ToString(); }
    }

    private readonly RealType _value;

    private PieceType(RealType type) 
      : this()
    {
      _value = type;
    }

    /// <summary>King</summary>
    public static readonly PieceType 玉 = new PieceType(RealType.K);
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
    /// <summary>Promoted Pawn</summary>
    public static readonly PieceType と = new PieceType(RealType.Pp);

    public static implicit operator string(PieceType type)
    {
      switch (type._value)
      {
        case RealType.K: return "玉";
        case RealType.R: return "飛";
        case RealType.B: return "角";
        case RealType.G: return "金";
        case RealType.S: return "銀";
        case RealType.N: return "桂";
        case RealType.L: return "香";
        case RealType.P: return "歩";
        case RealType.Rp: return "竜";
        case RealType.Bp: return "馬";
        case RealType.Sp: return "金";
        case RealType.Np: return "成";
        case RealType.Lp: return "成";
        case RealType.Pp: return "と";
      }
      throw new ArgumentOutOfRangeException("type");
    }
    public static implicit operator PieceType(string symbol)
    {
      if (symbol == null) throw new ArgumentNullException("symbol");
      switch (symbol)
      {
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
        case "K": return 玉;
        case "R": return 飛;
        case "B": return 角;
        case "G": return 金;
        case "S": return 銀;
        case "N": return 桂;
        case "L": return 香;
        case "P": return 歩;
        case "+R": return 竜;
        case "+B": return 馬;
        case "+S": return 金;
        case "+N": return new PieceType(RealType.Np);
        case "+L": return new PieceType(RealType.Lp);
        case "+P": return new PieceType(RealType.Pp);
      }
      throw new ArgumentOutOfRangeException("symbol");
    }

    public bool Equals(PieceType other)
    {
      return Equals(other._value, _value);
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (obj.GetType() != typeof (PieceType)) return false;
      return Equals((PieceType) obj);
    }
    public override int GetHashCode()
    {
      return _value.GetHashCode();
    }
    public static bool operator ==(PieceType left, PieceType right)
    {
      return left.Equals(right);
    }
    public static bool operator !=(PieceType left, PieceType right)
    {
      return !left.Equals(right);
    }

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

    public PieceType Unpromote()
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
      throw new InvalidOperationException("Can't promote " + this);
    }
    public override string ToString()
    {
      return this;
    }
  }
}