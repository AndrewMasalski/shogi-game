using System;
using System.Collections.Generic;

namespace Yasc.ShogiCore.Primitives
{
  /// <summary>Represents piece type</summary>
  public interface IPiece : IComparable<IPiece>
  {
    /// <summary>Piece color</summary>
    PieceColor Color { get; }

    /// <summary>Russian piece name</summary>
    string Russian { get; }
    /// <summary>English piece name</summary>
    string English { get; }
    /// <summary>Gets the latin piece symbol</summary>
    string Symbol { get; }
    /// <summary>Gets all directions of move piece can do</summary>
    IEnumerable<IMoveDirection> MoveDirections { get; }
  }
}