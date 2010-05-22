using System;
using System.Linq;
using System.Windows;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.UnitTests.ShogiPiece
{
  public partial class ShogiPieceTestControl
  {
    public ShogiPieceTestControl()
    {
      InitializeComponent();
      DataContext = this;
      var b = new Board();
      Pieces = new[] { b.PieceSet[PieceType.と], b.PieceSet[PieceType.Parse("玉")] };
      PossibleDirections = (PieceDirection[])Enum.GetValues(typeof(PieceDirection));
      PossibleColors = (PieceColor[])Enum.GetValues(typeof(PieceColor));
      PossibleTypes = PieceType.AllPieceTypes.ToArray();
 
    }

    public Piece[] Pieces { get; private set; }
    public PieceDirection[] PossibleDirections { get; private set; }
    public PieceColor[] PossibleColors { get; private set; }
    public IPieceType[] PossibleTypes { get; private set; }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        _piece.Piece = (Piece)_piecesCombo.SelectedValue;
      }
      catch (InvalidOperationException x)
      {
        MessageBox.Show(x.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
      try
      {
        _piece.Piece = null;
      }
      catch (InvalidOperationException x)
      {
        MessageBox.Show(x.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}
