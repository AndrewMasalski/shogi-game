using System;
using System.Windows;
using System.Windows.Automation.Peers;
using MvvmFoundation.Wpf;
using Yasc.Controls.Automation;
using Yasc.ShogiCore;
using Yasc.GenericDragDrop;

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
      if (cell == null || _cellObserver != null)
      {
        throw new InvalidOperationException(
          "You can set ShogiCell.Cell property just " +
          "once and with not null value only");
      }

      UpdateCp(cell);
      _cellObserver = new PropertyObserver<Cell>(cell).
        RegisterHandler(c => c.Piece, UpdateCp);
    }

    private void UpdateCp(Cell cell)
    {
      var piece = cell == null ? null : cell.Piece;
      Content = piece == null ? null : new ShogiPiece(cell.Piece);
    }

    public Cell Cell
    {
      get { return (Cell)GetValue(CellProperty); }
      set { SetValue(CellProperty, value); }
    }

    #endregion

    #region ' IsPromotionAllowed Property  '

    public static readonly DependencyProperty IsPromotionAllowedProperty =
      DependencyProperty.Register("IsPromotionAllowed", typeof(bool),
        typeof(PieceHolderBase), new UIPropertyMetadata(false));

    public bool IsPromotionAllowed
    {
      get { return (bool)GetValue(IsPromotionAllowedProperty); }
      set { SetValue(IsPromotionAllowedProperty, value); }
    }

    public static readonly DependencyProperty IsPromotionRecommendedProperty =
      DependencyProperty.Register("IsPromotionRecommended", typeof(bool),
        typeof(PieceHolderBase), new UIPropertyMetadata(false));

    public bool IsPromotionRecommended
    {
      get { return (bool)GetValue(IsPromotionRecommendedProperty); }
      set { SetValue(IsPromotionRecommendedProperty, value); }
    }

    #endregion

    protected override void UpdateHolderControl()
    {
      UpdateCp(Cell);
    }
    public override void OnApplyTemplate()
    {
      var core = TemplatedParent.FindAncestor<ShogiBoardCore>();
      if (core != null) core.SetupCell(this);
      base.OnApplyTemplate();
    }

    /// <summary>Holds the reference to prevent GC from collecting</summary>
    private PropertyObserver<Cell> _cellObserver;

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new ShogiCellAutomationPeer(this);
    }
  }
}
