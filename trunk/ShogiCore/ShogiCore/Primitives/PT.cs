using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Yasc.ShogiCore.Primitives
{
  /// <summary>Represents piece type</summary>
  public static class PT
  {
    #region ' Parse '

    private static readonly Dictionary<string, IPieceType> _parserDic;

    /// <summary>Parses a symbol into a PieceType.
    ///   Recognizes japanese hieroglyphs as well as latin symbols</summary>
    public static IPieceType Parse(string symbol)
    {
      if (symbol == null) throw new ArgumentNullException("symbol");
      IPieceType res;
      if (_parserDic.TryGetValue(symbol, out res))
        return res;
      throw new ArgumentOutOfRangeException("symbol");
    }

    /// <summary>Tries to parse a symbol into a PieceType.
    ///   Recognizes japanese hieroglyphs as well as latin symbols</summary>
    /// <returns>false if it couldn't parse the symbol</returns>
    public static bool TryParse(string symbol, out IPieceType pieceType)
    {
      return _parserDic.TryGetValue(symbol, out pieceType);
    }

    #endregion

    #region ' Public Constants '

    /// <summary>King (reigning)</summary>
    public static readonly IPieceType 王;
    /// <summary>King (challenging)</summary>
    public static readonly IPieceType 玉;
    /// <summary>Rook</summary>
    public static readonly IPieceType 飛;
    /// <summary>Bishop</summary>
    public static readonly IPieceType 角;
    /// <summary>Gold general</summary>   
    public static readonly IPieceType 金;
    /// <summary>Silver general</summary> 
    public static readonly IPieceType 銀;
    /// <summary>Knight</summary>         
    public static readonly IPieceType 桂;
    /// <summary>Lance</summary>          
    public static readonly IPieceType 香;
    /// <summary>Pawn</summary>           
    public static readonly IPieceType 歩;
    /// <summary>Promoted Rook</summary>  
    public static readonly IPieceType 竜;
    /// <summary>Promoted Bishop</summary>
    public static readonly IPieceType 馬;
    /// <summary>Promoted Silver</summary>
    public static readonly IPieceType 全;
    /// <summary>Promoted Pawn</summary>  
    public static readonly IPieceType と;
    /// <summary>Promoted Knight</summary>
    public static readonly IPieceType 今;
    /// <summary>Promoted Lance</summary> 
    public static readonly IPieceType 仝;

    /// <summary>King (reigning)</summary>
    public static readonly IPieceCategory Kr;
    /// <summary>King (challenging)</summary>
    public static readonly IPieceCategory Kc;
    /// <summary>King</summary>
    public static readonly IPieceCategory K;
    /// <summary>Rook</summary>
    public static readonly IPieceCategory R;
    /// <summary>Bishop</summary>
    public static readonly IPieceCategory B;
    /// <summary>Gold general</summary>   
    public static readonly IPieceCategory G;
    /// <summary>Silver general</summary> 
    public static readonly IPieceCategory S;
    /// <summary>Knight</summary>         
    public static readonly IPieceCategory N;
    /// <summary>Lance</summary>          
    public static readonly IPieceCategory L;
    /// <summary>Pawn</summary>           
    public static readonly IPieceCategory P;

    /// <summary>All Types pieces may have</summary>
    public static ReadOnlyCollection<IPieceType> AllPieceTypes { get; private set; }
    /// <summary>All Kinds pieces may have</summary>
    public static ReadOnlyCollection<IPieceCategory> AllPieceKinds { get; private set; }
    /// <summary>All Qualities pieces may have</summary>
    public static ReadOnlyCollection<IPieceCategory> AllPieceQualities { get; private set; }

    static PT()
    {
      //      ____________________
      // ____/ Create PieceType's \_______________________________________________________
      var kr = new PieceType(RealType.Kr, "王", "Kr", "");
      var kc = new PieceType(RealType.Kc, "玉", "Kc", "");
      var r = new PieceType(RealType. R , "飛", "R", "Хися");
      var b = new PieceType(RealType. B , "角", "B", "Какугё");
      var g = new PieceType(RealType. G , "金", "G", "Кин");
      var s = new PieceType(RealType. S , "銀", "S", "Гин");
      var n = new PieceType(RealType. N , "桂", "N", "Кэйма");
      var l = new PieceType(RealType. L , "香", "L", "Кёся");
      var p = new PieceType(RealType. P , "歩", "P", "Фухё");
      var rp = new PieceType(RealType.Rp, "竜", "Rp", "");
      var bp = new PieceType(RealType.Bp, "馬", "Bp", "");
      var sp = new PieceType(RealType.Sp, "全", "Sp", "");
      var pp = new PieceType(RealType.Pp, "と", "Pp", "");
      var np = new PieceType(RealType.Np, "今", "Np", "");
      var lp = new PieceType(RealType.Lp, "仝", "Lp", "");

      //      _______________________________
      // ____/ Promoted/Demoted connectivity \____________________________________________
      r.Promoted = rp; b.Promoted = bp; s.Promoted = sp; n.Promoted = np;
      l.Promoted = lp; p.Promoted = pp; rp.Demoted = r; bp.Demoted = b;
      sp.Demoted = s; np.Demoted = n; lp.Demoted = l; pp.Demoted = p;

      //      _____________________
      // ____/ Assign IPieceType's \______________________________________________________
      王 = kr; 玉 = kc; 飛 = r; 角 = b; 金 = g; 銀 = s; 桂 = n; 香 = l;
      歩 = p; 竜 = rp; 馬 = bp; 全 = sp; と = pp; 今 = np; 仝 = lp;

      //      ___________________
      // ____/ Create categories \________________________________________________________
      R = new PieceCategory(0, r, rp);
      B = new PieceCategory(1, b, bp);
      G = new PieceCategory(2, g);
      S = new PieceCategory(3, s, sp);
      N = new PieceCategory(4, n, np);
      L = new PieceCategory(5, l, lp);
      P = new PieceCategory(6, p, pp);
      K = new PieceCategory(7, kr, kc);
      Kr = new PieceCategory(7, kr);
      Kc = new PieceCategory(8, kc);

      //      ___________________
      // ____/ Assign categories \________________________________________________________
      kr.PieceKind = K;
      kr.PieceQuality = Kr;
      kc.PieceKind = K;
      kc.PieceQuality = Kc;
      r.PieceKind = r.PieceQuality = R;
      b.PieceKind = b.PieceQuality = B;
      g.PieceKind = g.PieceQuality = G;
      s.PieceKind = s.PieceQuality = S;
      n.PieceKind = n.PieceQuality = N;
      l.PieceKind = l.PieceQuality = L;
      p.PieceKind = p.PieceQuality = P;
      rp.PieceKind = rp.PieceQuality = R;
      bp.PieceKind = bp.PieceQuality = B;
      sp.PieceKind = sp.PieceQuality = S;
      pp.PieceKind = pp.PieceQuality = P;
      np.PieceKind = np.PieceQuality = N;
      lp.PieceKind = lp.PieceQuality = L;

      //      ______________
      // ____/ Dictionaries \_____________________________________________________________
      _parserDic = new Dictionary<string, IPieceType>
        {
          {"王", 王}, {"玉", 玉}, {"飛", 飛}, {"角", 角}, {"金", 金}, {"銀", 銀}, {"桂", 桂}, {"香", 香}, 
          {"歩", 歩}, {"竜", 竜}, {"馬", 馬}, {"全", 全}, {"今", 今}, {"仝", 仝}, {"と", と}, 
          {"Kr", 王}, {"Kc", 玉}, {"R", 飛}, {"B", 角}, {"G", 金}, {"S", 銀}, {"N", 桂}, {"L", 香}, 
          {"P", 歩}, {"+R", 竜}, {"+B", 馬}, {"+S", 全}, {"+N", 今}, {"+L", 仝}, {"+P", と},
        };

      _deserializerDic = new Dictionary<RealType, IPieceType>(15)
          {
            {RealType.Kr, 王}, {RealType.Kc, 玉}, {RealType.R, 飛}, {RealType.B, 角}, {RealType.G, 金},
            {RealType.S, 銀}, {RealType.N, 桂}, {RealType.L, 香}, {RealType.P, 歩}, {RealType.Rp, 竜},
            {RealType.Bp, 馬}, {RealType.Sp, 全}, {RealType.Pp, と}, {RealType.Np, 今}, {RealType.Lp, 仝}
          };

      //      _____________
      // ____/ Collections \______________________________________________________________
      AllPieceTypes = new ReadOnlyCollection<IPieceType>(
        new List<IPieceType>(14)
          {
            王, 玉, 飛, 角, 金, 銀, 桂, 香, 歩, 竜, 馬, 全, と, 今, 仝,
          }); 
      AllPieceKinds = new ReadOnlyCollection<IPieceCategory>(
        new List<IPieceCategory>(8)
          {
            K, R, B, G, S, N, L, P,
          });
      AllPieceQualities = new ReadOnlyCollection<IPieceCategory>(
        new List<IPieceCategory>(9)
          {
            Kr, Kc, R, B, G, S, N, L, P,
          });

      //      ________________
      // ____/ MoveDircetions \___________________________________________________________
      const int max = 8;

      kr.MoveDirections = Join(Up(1), UpRight(1), Right(1), DownRight(1), Down(1), DownLeft(1), Left(1), UpLeft(1));
      kc.MoveDirections = kr.MoveDirections;
      r.MoveDirections = Join(Up(max), Right(max), Down(max), Left(max));
      b.MoveDirections = Join(UpRight(max), DownRight(max), DownLeft(max), UpLeft(max));
      g.MoveDirections = Join(Up(1), UpRight(1), Right(1), Down(1), Left(1), UpLeft(1));
      s.MoveDirections = Join(Up(1), UpRight(1), DownRight(1), DownLeft(1), UpLeft(1));
      n.MoveDirections = Join(Go(1, 2, 1), Go(-1, 2, 1));
      l.MoveDirections = Join(Up(max));
      p.MoveDirections = Join(Up(1));
      rp.MoveDirections = Join(r.MoveDirections, UpLeft(1), UpRight(1), DownRight(1), DownLeft(1));
      bp.MoveDirections = Join(b.MoveDirections, Up(1), Right(1), Down(1), Left(1));
      sp.MoveDirections = g.MoveDirections;
      pp.MoveDirections = g.MoveDirections;
      np.MoveDirections = g.MoveDirections;
      lp.MoveDirections = g.MoveDirections;
    }

    #endregion

    #region ' RealType, PieceCategory, PieceType '

    private enum RealType
    {
      Kr, Kc, R, B, G, S, N, L, P, Rp, Bp, Sp, Np, Lp, Pp
    }
    private sealed class PieceCategory : IPieceCategory
    {
      public ReadOnlyCollection<IPieceType> PieceTypes { get; private set; }
      public int Id { get; private set; }

      public PieceCategory(int id, params PieceType[] types)
      {
        Id = id;
        PieceTypes = new ReadOnlyCollection<IPieceType>(types);
      }

      public int CompareTo(IPieceCategory other)
      {
        return PieceTypes[0].CompareTo(other.PieceTypes[0]);
      }
    }

    [Serializable]
    private sealed class PieceType : IPieceType, ISerializable
    {
      private readonly RealType _realType;
      public string Japanese { get; private set; }
      public string Russian { get; private set; }
      public IPieceType Promoted { get; set; }
      public IPieceType Demoted { get; set; }
      public string Latin { get; private set; }
      public bool IsPromoted { get { return Demoted != null; } }
      public bool CanPromote { get { return Promoted != null; } }
      public IPieceCategory PieceKind { get; set; }
      public IPieceCategory PieceQuality { get; set; }
      public IEnumerable<IMoveDirection> MoveDirections { get; set; }

      public PieceType(RealType realType, string japanese, string latin, string russian)
      {
        _realType = realType;
        Japanese = japanese;
        Latin = latin;
        Russian = russian;
      }

      public IPieceType Promote()
      {
        if (Promoted != null) return Promoted;
        throw new InvalidOperationException("Can't promote " + this);
      }
      public IPieceType Demote()
      {
        if (Demoted != null) return Demoted;
        throw new InvalidOperationException("Can't demote " + this);
      }
      public int CompareTo(IPieceType other)
      {
        return (int)_realType - (int)((PieceType)other)._realType;
      }
      public override string ToString()
      {
        return Japanese;
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
        info.AddValue("RealType", _realType);
        // Instead of serializing this object, 
        // serialize a PieceTypeSerializationHelper instead.
        info.SetType(typeof(PieceTypeSerializationHelper));
      }
    }

    private static readonly Dictionary<RealType, IPieceType> _deserializerDic;

    [Serializable]
    private sealed class PieceTypeSerializationHelper : IObjectReference, ISerializable
    {
      private readonly RealType _realType;

      private PieceTypeSerializationHelper(SerializationInfo info, StreamingContext context)
      {
        _realType = (RealType)info.GetValue("RealType", typeof(RealType));
      }

      public Object GetRealObject(StreamingContext context)
      {
        return _deserializerDic[_realType];
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
      }
    }

    #endregion

    #region ' MoveDirection '

    private sealed class MoveDirection : IMoveDirection
    {
      public int Dx { get; private set; }
      public int Dy { get; private set; }
      public int Count { get; private set; }

      public MoveDirection(int dx, int dy, int count)
      {
        Dx = dx;
        Dy = dy;
        Count = count;
      }
    }

    private static IEnumerable<T> Join<T>(params IEnumerable<T>[] arr)
    {
      return arr.SelectMany(e => e).ToArray();
    }

    private static IEnumerable<MoveDirection> Up(int i)
    {
      return Go(0, 1, i);
    }
    private static IEnumerable<MoveDirection> UpRight(int i)
    {
      return Go(1, 1, i);
    }
    private static IEnumerable<MoveDirection> Right(int i)
    {
      return Go(1, 0, i);
    }
    private static IEnumerable<MoveDirection> DownRight(int i)
    {
      return Go(1, -1, i);
    }
    private static IEnumerable<MoveDirection> Down(int i)
    {
      return Go(0, -1, i);
    }
    private static IEnumerable<MoveDirection> DownLeft(int i)
    {
      return Go(-1, -1, i);
    }
    private static IEnumerable<MoveDirection> Left(int i)
    {
      return Go(-1, 0, i);
    }
    private static IEnumerable<MoveDirection> UpLeft(int i)
    {
      return Go(-1, 1, i);
    }

    private static IEnumerable<MoveDirection> Go(int dx, int dy, int count)
    {
      yield return new MoveDirection(dx, dy, count);

    }

    #endregion
  }
}