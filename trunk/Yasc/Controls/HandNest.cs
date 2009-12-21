using System.Windows;
using System.Windows.Automation.Peers;
using Yasc.Controls.Automation;
using Yasc.ShogiCore;

namespace Yasc.Controls
{
  public class HandNest : PieceHolderBase
  {
    static HandNest()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HandNest),
        new FrameworkPropertyMetadata(typeof(HandNest)));
    }

    public HandNest()
    {
      UpdateHolderControl();
    }

    private void UpdateHolderControl(PieceType pieceType, PieceColor pieceColor)
    {
      Content = new ShogiPiece { PieceType = pieceType, PieceColor = pieceColor };
    }
    protected override void UpdateHolderControl()
    {
      UpdateHolderControl(PieceType, PieceColor);
    }
    protected override void OnPieceColorChanged(PieceColor pieceColor)
    {
      UpdateHolderControl(PieceType, pieceColor);
      base.OnPieceColorChanged(pieceColor);
    }
    public override ShogiPiece DetachPiece()
    {
      var res = base.DetachPiece();
      if (PiecesCount > 1) UpdateHolderControl(PieceType, PieceColor);
      return res;
    }

    #region ' PiecesCount Property '

    public static readonly DependencyProperty PiecesCountProperty =
      DependencyProperty.Register("PiecesCount", typeof(int),
        typeof(HandNest), new UIPropertyMetadata(0));

    public int PiecesCount
    {
      get { return (int)GetValue(PiecesCountProperty); }
      set { SetValue(PiecesCountProperty, value); }
    }

    #endregion

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new HandNestAutomationPeer(this);
    }
  }
}
