using System;
using System.Collections.Generic;

namespace Yasc.ShogiCore.Primitives
{
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
    ///   promoted and unpromoted versions and different kings</summary>
    IPieceCategory PieceKind { get; }
    ///<summary>Gets the kind of the piece which is the same for 
    ///   promoted and unpromoted versions and NOT the same for different kings</summary>
    IPieceCategory PieceQuality { get; }
    /// <summary>Gets all directions of move piece can do</summary>
    IEnumerable<IMoveDirection> MoveDirections { get; }
  }
}