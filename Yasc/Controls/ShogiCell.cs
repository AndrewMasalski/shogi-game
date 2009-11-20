using System.Windows;
using System.Windows.Controls;
using MvvmFoundation.Wpf;
using Yasc.ShogiCore.Utils;

namespace Yasc.Controls
{
  [TemplatePart(Name = "PART_Piece", Type = typeof(ContentPresenter))]
  public class ShogiCell : Control
  {
    static ShogiCell()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiCell),
        new FrameworkPropertyMetadata(typeof(ShogiCell)));
    }

    #region 'IsPossible MoveTarget '

    public static readonly DependencyProperty IsPossibleMoveTargetProperty =
      DependencyProperty.Register("IsPossibleMoveTarget", typeof(bool),
                                  typeof(ShogiCell), new UIPropertyMetadata(false));

    public bool IsPossibleMoveTarget
    {
      get { return (bool)GetValue(IsPossibleMoveTargetProperty); }
      set { SetValue(IsPossibleMoveTargetProperty, value); }
    }

    #endregion

    #region ' Cell Property '

    public static readonly DependencyProperty CellProperty =
      DependencyProperty.Register("Cell", typeof (Cell),
        typeof (ShogiCell), new UIPropertyMetadata(null, OnCellChanged));

    private static void OnCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

      ((ShogiCell) d).OnCellChanged((Cell) e.NewValue);
    }

    private void OnCellChanged(Cell cell)
    {
        UpdateCp(cell);
      _cellObserver = new PropertyObserver<Cell>(cell).
        RegisterHandler(c => c.Piece, UpdateCp);
    }


    private void UpdateCp(Cell cell)
    {
      if (_cp == null) return;
      _cp.Content = cell == null || cell.Piece == null ? null : new ShogiPiece(cell.Piece);
    }

    public Cell Cell
    {
      get { return (Cell) GetValue(CellProperty); }
      set { SetValue(CellProperty, value); }
    }

    #endregion

    #region ' IsMoveSource Property '

    public static readonly DependencyProperty IsMoveSourceProperty =
      DependencyProperty.Register("IsMoveSource", typeof (bool),
       typeof(ShogiCell), new UIPropertyMetadata(false));


    public bool IsMoveSource
    {
      get { return (bool) GetValue(IsMoveSourceProperty); }
      set { SetValue(IsMoveSourceProperty, value); }
    }

    #endregion

    #region ' IsFlipped Property '

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.IsFlippedProperty.AddOwner(
      typeof(ShogiCell), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public bool IsFlipped
    {                            
      get { return (bool)GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    #endregion


    public ShogiPiece ShogiPiece
    {
      get { return _cp != null ? (ShogiPiece)_cp.Content : null;  }
    }

    public override void OnApplyTemplate()
    {
        _cp = GetTemplateChild("PART_Piece") as ContentPresenter;
        UpdateCp(Cell);
      base.OnApplyTemplate();
    }
    private ContentPresenter _cp;

// ReSharper disable UnaccessedField.Local
    // We need it for GC not to collect it
    private PropertyObserver<Cell> _cellObserver;
// ReSharper restore UnaccessedField.Local
  }
}
