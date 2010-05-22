using System.Windows;
using System.Windows.Automation.Peers;
using Yasc.BoardControl.Controls.Automation;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace Yasc.BoardControl.Controls
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
      UpdateContent();
    }

    private void UpdateContent(IPieceType pieceType, PieceColor pieceColor)
    {
      Content = new ShogiPiece { PieceType = pieceType, PieceColor = pieceColor };
    }
    protected override void UpdateContent()
    {
      UpdateContent(PieceType, PieceColor);
    }
    protected override void OnPieceColorChanged(PieceColor pieceColor)
    {
      UpdateContent(PieceType, pieceColor);
      base.OnPieceColorChanged(pieceColor);
    }
    public override ShogiPiece DetachPiece()
    {
      var res = base.DetachPiece();
      if (PiecesCount > 1) UpdateContent(PieceType, PieceColor);
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
