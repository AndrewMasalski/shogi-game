using System.Windows;
using MvvmFoundation.Wpf;
using Yasc.ShogiCore.Utils;

namespace Yasc.Controls
{
  public class ShogiCell : PieceHolderBase
  {
    static ShogiCell()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiCell),
        new FrameworkPropertyMetadata(typeof(ShogiCell)));
    }

    #region ' Cell Property '

    public static readonly DependencyProperty CellProperty =
      DependencyProperty.Register("Cell", typeof(Cell),
        typeof(ShogiCell), new UIPropertyMetadata(null, OnCellChanged));

    private static void OnCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

      ((ShogiCell)d).OnCellChanged((Cell)e.NewValue);
    }

    private void OnCellChanged(Cell cell)
    {
      UpdateCp(cell);
      _cellObserver = new PropertyObserver<Cell>(cell).
        RegisterHandler(c => c.Piece, UpdateCp);
    }


    private void UpdateCp(Cell cell)
    {
      if (Cp == null) return;
      Cp.Content = cell == null || cell.Piece == null ? null : new ShogiPiece(cell.Piece);
    }

    public Cell Cell
    {
      get { return (Cell)GetValue(CellProperty); }
      set { SetValue(CellProperty, value); }
    }

    #endregion

    protected override void UpdateCp()
    {
      UpdateCp(Cell);
    }
    
// ReSharper disable UnaccessedField.Local
    // We need it for GC not to collect it
    private PropertyObserver<Cell> _cellObserver;
// ReSharper restore UnaccessedField.Local
  }
}
