using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Yasc.ShogiCore.Primitives;

namespace Chess
{
  /// <summary>Represents piece type</summary>
  public static class Pt
  {
    #region ' Parse '

    private static readonly Dictionary<string, IPiece> ParserDic;

    /// <summary>Parses a symbol into a Piece.
    ///   Recognizes japanese hieroglyphs as well as latin symbols</summary>
    public static IPiece Parse(string symbol)
    {
      if (symbol == null) throw new ArgumentNullException("symbol");
      IPiece res;
      if (ParserDic.TryGetValue(symbol, out res))
        return res;
      throw new ArgumentOutOfRangeException("symbol");
    }

    /// <summary>Tries to parse a symbol into a PieceType.
    ///   Recognizes japanese hieroglyphs as well as latin symbols</summary>
    /// <returns>false if it couldn't parse the symbol</returns>
    public static bool TryParse(string symbol, out IPiece piece)
    {
      return ParserDic.TryGetValue(symbol, out piece);
    }

    #endregion

    #region ' Public Constants '

    public static readonly IPiece P;
    public static readonly IPiece B;
    public static readonly IPiece N;
    public static readonly IPiece R;
    public static readonly IPiece Q;
    /// <summary>White King</summary>
    public static readonly IPiece K;

// ReSharper disable InconsistentNaming
    public static readonly IPiece p;
    public static readonly IPiece b;
    public static readonly IPiece n;
    public static readonly IPiece r;
    public static readonly IPiece q;
    /// <summary>Black King</summary>
    public static readonly IPiece k;
    // ReSharper restore InconsistentNaming


    /// <summary>All Types pieces may have</summary>
    public static ReadOnlyCollection<IPiece> AllPieceTypes { get; private set; }

    static Pt()
    {
      //      _______________
      // ____/ Create Pieces \____________________________________________________________
      var wp = new Piece(RealPieceType.WhitePawn, "P", "White Pawn", "Белая пешка", PieceColor.White);
      var wb = new Piece(RealPieceType.WhiteBishop, "B", "White Bishop", "Белый слон", PieceColor.White);
      var wn = new Piece(RealPieceType.WhiteKnight, "N", "White Knight", "Белый конь", PieceColor.White);
      var wr = new Piece(RealPieceType.WhiteRook, "R", "White Rook", "Белая ладья", PieceColor.White);
      var wq = new Piece(RealPieceType.WhiteQueen, "Q", "White Queen", "Белый ферзь", PieceColor.White);
      var wk = new Piece(RealPieceType.WhiteKing, "K", "White King", "Белый король", PieceColor.White);

      var bp = new Piece(RealPieceType.BlackPawn, "P", "Black Pawn", "Черная пешка", PieceColor.Black);
      var bb = new Piece(RealPieceType.BlackBishop, "B", "Black Bishop", "Черный слон", PieceColor.Black);
      var bn = new Piece(RealPieceType.BlackKnight, "N", "Black Knight", "Черный конь", PieceColor.Black);
      var br = new Piece(RealPieceType.BlackRook, "R", "Black Rook", "Черная ладья", PieceColor.Black);
      var bq = new Piece(RealPieceType.BlackQueen, "Q", "Black Queen", "Черный ферзь", PieceColor.Black);
      var bk = new Piece(RealPieceType.BlackKing, "K", "Black King", "Черный король", PieceColor.Black);

      var cc = new[] {wp, wb, wn, wr, wq, wk, bp, bb, bn, br, bq, bk};

      //      _____________
      // ____/ Collections \______________________________________________________________
      AllPieceTypes = new ReadOnlyCollection<IPiece>(cc);

      P = wp; B = wb; N = wn; R = wr; Q = wq; K = wk;
      p = bp; b = bb; n = bn; r = br; q = bq; k = bk;

      //      ______________
      // ____/ Dictionaries \_____________________________________________________________
      ParserDic = AllPieceTypes.ToDictionary(i => i.Symbol, i => i);
      DeserializerDic = cc.ToDictionary(i => i.RealType, i => i);

      //      ________________
      // ____/ MoveDircetions \___________________________________________________________
      const int max = 8;

      wk.MoveDirections = Join(Up(1), UpRight(1), Right(1), DownRight(1), Down(1), DownLeft(1), Left(1), UpLeft(1));
      wr.MoveDirections = Join(Up(max), Right(max), Down(max), Left(max));
      wb.MoveDirections = Join(UpRight(max), DownRight(max), DownLeft(max), UpLeft(max));
      wn.MoveDirections = Join(Go(1, 2, 1), Go(-1, 2, 1), Go(1, -2, 1), Go(-1, -2, 1),
                               Go(2, 1, 1), Go(-2, 1, 1), Go(2, -1, 1), Go(-2, -1, 1));
      wq.MoveDirections = Join(R.MoveDirections, B.MoveDirections);

      bk.MoveDirections = wk.MoveDirections;
      br.MoveDirections = wr.MoveDirections;
      bb.MoveDirections = wb.MoveDirections;
      bn.MoveDirections = wn.MoveDirections;
      bq.MoveDirections = wq.MoveDirections;
    }

    #endregion

    #region ' RealPieceType, PieceCategory, PieceType, ColoredPiece '

    private enum RealPieceType
    {
      WhitePawn,
      WhiteBishop,
      WhiteKnight,
      WhiteRook,
      WhiteQueen,
      WhiteKing,

      BlackPawn,
      BlackBishop,
      BlackKnight,
      BlackRook,
      BlackQueen,
      BlackKing,
    }

    [Serializable]
    private sealed class Piece : IPiece, ISerializable
    {
      public RealPieceType RealType { get; private set; }
      public PieceColor Color { get; private set; }
      public string Russian { get; private set; }
      public string Symbol { get; private set; }
      public string English { get; private set; }
      public IEnumerable<IMoveDirection> MoveDirections { get; set; }

      public Piece(RealPieceType realType, string symbol, string english, string russian, PieceColor color)
      {
        RealType = realType;
        Symbol = symbol;
        English = english;
        Russian = russian;
        Color = color;
      }

      public int CompareTo(IPiece other)
      {
        return (int)RealType - (int)((Piece)other).RealType;
      }
      public override string ToString()
      {
        return Symbol;
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
        info.AddValue("RealType", RealType);
        // Instead of serializing this object, 
        // serialize a PieceTypeSerializationHelper instead.
        info.SetType(typeof(PieceTypeSerializationHelper));
      }
    }

    private static readonly Dictionary<RealPieceType, Piece> DeserializerDic;

    [Serializable]
    private sealed class PieceTypeSerializationHelper : IObjectReference, ISerializable
    {
      private readonly RealPieceType _realType;

      private PieceTypeSerializationHelper(SerializationInfo info, StreamingContext context)
      {
        _realType = (RealPieceType)info.GetValue("RealType", typeof(RealPieceType));
      }

      public Object GetRealObject(StreamingContext context)
      {
        return DeserializerDic[_realType];
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