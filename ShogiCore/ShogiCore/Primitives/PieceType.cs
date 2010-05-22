using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Yasc.ShogiCore.Primitives
{
  /// <summary>Kind of the piece which is the same for 
  ///   promoted and unpromoted versions</summary>
  public interface IPieceCategory : IComparable<IPieceCategory>
  {
    /// <summary><see cref="IPieceType"/>s associated with the <see cref="IPieceCategory"/></summary>
    ReadOnlyCollection<IPieceType> PieceTypes { get; }
    ///<summary>Every <see cref="IPieceCategory"/> has integer ID from [0..7]</summary>
    int Id { get; }
  }
  /// <summary>Represents piece type</summary>
  public interface IPieceType : IComparable<IPieceType>
  {
    /// <summary>Japanese sybmol short name</summary>
    string Japanese { get; }
    /// <summary>Reference to the promoted version of the piece -or- null</summary>
    IPieceType Promoted { get; }
    /// <summary>Reference to the demoted version of the piece -or- null</summary>
    IPieceType Demoted { get; }
    /// <summary>Russian sybmol short name</summary>
    string Russian { get; }
    /// <summary>Indicates wheter the piece type is promoted</summary>
    bool IsPromoted { get; }
    /// <summary>Indicates whether the piece type can be promoted</summary>
    bool CanPromote { get; }
    /// <summary>Gets the latin version of piece type</summary>
    string Latin { get; }
    /// <summary>Returns promoted version of the piece type</summary>
    IPieceType Promote();
    /// <summary>Returns "unpromoted" version of the piece type</summary>
    IPieceType Demote();
    /// <summary>Gets the kind of the piece which is the same for 
    ///   promoted and unpromoted versions + different kings</summary>
    IPieceCategory PieceKind { get; }
    ///<summary>Gets the kind of the piece which is the same for 
    ///   promoted and unpromoted versions and NOT different kings</summary>
    IPieceCategory PieceQuality { get; }
  }
  /// <summary>Represents piece type</summary>
  public static class PieceType
  {
    #region ' Parse '

    private static readonly Dictionary<string, IPieceType> _parser;

    /// <summary>Parses a symbol into a PieceType.
    ///   Recognizes japanese hieroglyphs as well as latin symbols</summary>
    public static IPieceType Parse(string symbol)
    {
      if (symbol == null) throw new ArgumentNullException("symbol");
      IPieceType res;
      if (_parser.TryGetValue(symbol, out res))
        return res;
      throw new ArgumentOutOfRangeException("symbol");
    }

    /// <summary>Tries to parse a symbol into a PieceType.
    ///   Recognizes japanese hieroglyphs as well as latin symbols</summary>
    /// <returns>false if it couldn't parse the symbol</returns>
    public static bool Parse(string symbol, out IPieceType pieceType)
    {
      if (symbol == null) throw new ArgumentNullException("symbol");
      return _parser.TryGetValue(symbol, out pieceType);
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

    static PieceType()
    {
      var kr = new PieceTypeImpl(RealType.Kr, "王", "Kr", "");
      var kc = new PieceTypeImpl(RealType.Kc, "玉", "Kc", "");
      var r = new PieceTypeImpl(RealType.R, "飛", "R", "Хися");
      var b = new PieceTypeImpl(RealType.B, "角", "B", "Какугё");
      var g = new PieceTypeImpl(RealType.G, "金", "G", "Кин");
      var s = new PieceTypeImpl(RealType.S, "銀", "S", "Гин");
      var n = new PieceTypeImpl(RealType.N, "桂", "N", "Кэйма");
      var l = new PieceTypeImpl(RealType.L, "香", "L", "Кёся");
      var p = new PieceTypeImpl(RealType.P, "歩", "P", "Фухё");
      var rp = new PieceTypeImpl(RealType.Rp, "竜", "Rp", "");
      var bp = new PieceTypeImpl(RealType.Bp, "馬", "Bp", "");
      var sp = new PieceTypeImpl(RealType.Sp, "全", "Sp", "");
      var pp = new PieceTypeImpl(RealType.Pp, "と", "Pp", "");
      var np = new PieceTypeImpl(RealType.Np, "今", "Np", "");
      var lp = new PieceTypeImpl(RealType.Lp, "仝", "Lp", "");

      r.Promoted = rp; b.Promoted = bp; s.Promoted = sp;
      n.Promoted = np; l.Promoted = lp; p.Promoted = pp;

      rp.Demoted = r; bp.Demoted = b; sp.Demoted = s;
      np.Demoted = n; lp.Demoted = l; pp.Demoted = p;

      王 = kr; 玉 = kc; 飛 = r; 角 = b;
      金 = g; 銀 = s; 桂 = n; 香 = l; 歩 = p;
      竜 = rp; 馬 = bp; 全 = sp; と = pp; 今 = np; 仝 = lp;

      AllPieceTypes = new ReadOnlyCollection<IPieceType>(
        new List<IPieceType>(14)
            {
              王, 玉, 飛, 角, 金, 銀, 桂, 香, 歩, 竜, 馬, 全, と, 今, 仝,
            });

      R = new PieceCategoryImpl(0, r, rp);
      B = new PieceCategoryImpl(1, b, bp);
      G = new PieceCategoryImpl(2, g);
      S = new PieceCategoryImpl(3, s, sp);
      N = new PieceCategoryImpl(4, n, np);
      L = new PieceCategoryImpl(5, l, lp);
      P = new PieceCategoryImpl(6, p, pp);
      K = new PieceCategoryImpl(7, kr, kc);
      Kr = new PieceCategoryImpl(7, kr);
      Kc = new PieceCategoryImpl(8, kc);

      kr.PieceKind = K; kr.PieceQuality = Kr;
      kc.PieceKind = K; kc.PieceQuality = Kc;
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

      _parser = new Dictionary<string, IPieceType>
          {
            {"王", 王}, {"玉", 玉}, {"飛", 飛}, {"角", 角}, {"金", 金}, {"銀", 銀}, {"桂", 桂}, {"香", 香}, {"歩", 歩},
            {"竜", 竜}, {"馬", 馬}, {"全", 全}, {"今", 今}, {"仝", 仝}, {"と", と},
            {"Kr", 王}, {"Kc", 玉}, {"R", 飛}, {"B", 角}, {"G", 金}, {"S", 銀}, {"N", 桂}, {"L", 香}, {"P", 歩}, 
            {"+R", 竜}, {"+B", 馬}, {"+S", 全}, {"+N", 今}, {"+L", 仝}, {"+P", と}, 
          };

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
    }

    #endregion

    #region ' Netseted Classes '

    private enum RealType
    {
      Kr, Kc, R, B, G, S, N, L, P, Rp, Bp, Sp, Np, Lp, Pp
    }

    [Serializable]
    private class PieceCategoryImpl : IPieceCategory
    {
      public PieceCategoryImpl(int id, params PieceTypeImpl[] types)
      {
        Id = id;
        PieceTypes = new ReadOnlyCollection<IPieceType>(types);
      }

      public ReadOnlyCollection<IPieceType> PieceTypes { get; private set; }

      public int Id { get; private set; }

      public int CompareTo(IPieceCategory other)
      {
        return PieceTypes[0].CompareTo(other.PieceTypes[0]);
      }
    }

    [Serializable]
    private class PieceTypeImpl : IPieceType
    {
      private readonly RealType _realType;
      public string Japanese { get; private set; }
      public string Russian { get; private set; }
      public IPieceType Promoted { get; set; }
      public IPieceType Demoted { get; set; }
      /// <summary>Gets the latin version of piece type</summary>
      public string Latin { get; private set; }
      /// <summary>Indicates wheter the piece type is promoted</summary>
      public bool IsPromoted { get { return Demoted != null; } }
      /// <summary>Indicates whether the piece type can be promoted</summary>
      public bool CanPromote { get { return Promoted != null; } }
      /// <summary>Returns promoted version of the piece type</summary>
      public IPieceType Promote()
      {
        if (Promoted != null) return Promoted;
        throw new InvalidOperationException("Can't promote " + this);
      }
      /// <summary>Returns "unpromoted" version of the piece type</summary>
      public IPieceType Demote()
      {
        if (Demoted != null) return Demoted;
        throw new InvalidOperationException("Can't demote " + this);
      }

      /// <summary>Gets the kind of the piece which is the same for 
      ///   promoted and unpromoted versions</summary>
      public IPieceCategory PieceKind { get; set; }

      public IPieceCategory PieceQuality { get; set; }

      public PieceTypeImpl(RealType realType, string japanese, string latin, string russian)
      {
        _realType = realType;
        Japanese = japanese;
        Latin = latin;
        Russian = russian;
      }

      public int CompareTo(IPieceType other)
      {
        return (int)_realType - (int)((PieceTypeImpl)other)._realType;
      }
      public override string ToString()
      {
        return Japanese;
      }
    }

    #endregion
  }
}