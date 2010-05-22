using System;
using System.Collections.ObjectModel;

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
}