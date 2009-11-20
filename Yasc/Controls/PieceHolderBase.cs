using System.Windows;
using System.Windows.Controls;

namespace Yasc.Controls
{
  [TemplatePart(Name = "PART_Piece", Type = typeof(ContentPresenter))]
  public abstract class PieceHolderBase : Control
  {
    #region 'IsPossible MoveTarget '

    public static readonly DependencyProperty IsPossibleMoveTargetProperty =
      DependencyProperty.Register("IsPossibleMoveTarget", typeof(bool),
                                  typeof(PieceHolderBase), new UIPropertyMetadata(false));

    public bool IsPossibleMoveTarget
    {
      get { return (bool)GetValue(IsPossibleMoveTargetProperty); }
      set { SetValue(IsPossibleMoveTargetProperty, value); }
    }

    #endregion

    #region ' IsMoveSource Property '

    public static readonly DependencyProperty IsMoveSourceProperty =
      DependencyProperty.Register("IsMoveSource", typeof(bool),
                                  typeof(PieceHolderBase), new UIPropertyMetadata(false));


    public bool IsMoveSource
    {
      get { return (bool)GetValue(IsMoveSourceProperty); }
      set { SetValue(IsMoveSourceProperty, value); }
    }

    #endregion

    #region ' IsFlipped Property '

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.IsFlippedProperty.AddOwner(
      typeof(PieceHolderBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public bool IsFlipped
    {
      get { return (bool)GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    #endregion
    
    static PieceHolderBase()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PieceHolderBase),
         new FrameworkPropertyMetadata(typeof(PieceHolderBase)));
    }
    public override void OnApplyTemplate()
    {
      Cp = GetTemplateChild("PART_Piece") as ContentPresenter;
      UpdateCp();
      base.OnApplyTemplate();
    }
    protected abstract void UpdateCp();

    public ShogiPiece DeattachPiece()
    {
      if (Cp == null) return null;
      var piece = ShogiPiece;
      Cp.Content = null;
      return piece;
    }
    public ContentPresenter Cp { get; private set; }
    public ShogiPiece ShogiPiece
    {
      get { return Cp != null ? (ShogiPiece)Cp.Content : null; }
    }
  }
}