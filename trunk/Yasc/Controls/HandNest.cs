using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    #region ' Hand Property '

    public static readonly DependencyProperty HandProperty =
      DependencyProperty.Register("Hand", typeof(ObservableCollection<Piece>),
        typeof(HandNest), new UIPropertyMetadata(null, OnHandChanged));

    private static void OnHandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

      ((HandNest)d).OnHandChanged((ObservableCollection<Piece>)e.OldValue, (ObservableCollection<Piece>)e.NewValue);
    }

    private void OnHandChanged(INotifyCollectionChanged oldCollection, ObservableCollection<Piece> newCollection)
    {
      if (oldCollection != null)
      {
        oldCollection.CollectionChanged -= OnHandCollectionChanged;
      }
      if (newCollection != null)
      {
        newCollection.CollectionChanged += OnHandCollectionChanged;
      }
      PiecesCount = newCollection == null ? 0 : Count(newCollection, PieceType);
    }

    private static int Count(IEnumerable collection, PieceType pieceType)
    {
      int counter = 0;
      foreach (Piece piece in collection)
        if (piece.Type == pieceType)
          counter++;
      return counter;
    }

    private void OnHandCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          PiecesCount += Count(args.NewItems, PieceType);
          break;
        case NotifyCollectionChangedAction.Remove:
          PiecesCount -= Count(args.NewItems, PieceType);
          break;
      }
    }

    public ObservableCollection<Piece> Hand
    {
      get { return (ObservableCollection<Piece>)GetValue(HandProperty); }
      set { SetValue(HandProperty, value); }
    }

    #endregion

    private void UpdateCp(PieceType pieceType, PieceColor pieceColor)
    {
      if (HolderControl == null) return;
      HolderControl.Content = new ShogiPiece { PieceType = pieceType, PieceColor = pieceColor };
    }
    protected override void UpdateHolderControl()
    {
      UpdateCp(PieceType, PieceColor);
    }
    protected override void OnPieceTypeChanged(PieceType pieceType)
    {
      if (Hand == null) return;
      PiecesCount = Count(Hand, pieceType);
      UpdateCp(pieceType, PieceColor);
      base.OnPieceTypeChanged(pieceType);
    }
    protected override void OnPieceColorChanged(PieceColor pieceColor)
    {
      UpdateCp(PieceType, pieceColor);
      base.OnPieceColorChanged(pieceColor);
    }
    public override ShogiPiece DetachPiece()
    {
      var res = base.DetachPiece();
      if (PiecesCount > 1) UpdateCp(PieceType, PieceColor);
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
