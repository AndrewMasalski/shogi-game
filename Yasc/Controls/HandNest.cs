using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

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
      if (Cp == null) return;
      Cp.Content = new ShogiPiece { PieceType = pieceType, PieceColor = pieceColor };
    }
    protected override void UpdateCp()
    {
      UpdateCp(PieceType, PieceColor);
    }

    #region ' PiecesCount Property '

    public static readonly DependencyProperty PiecesCountProperty =
      DependencyProperty.Register("PiecesCount", typeof (int),
        typeof (HandNest), new UIPropertyMetadata(0));

    public int PiecesCount
    {
      get { return (int) GetValue(PiecesCountProperty); }
      set { SetValue(PiecesCountProperty, value); }
    }

    #endregion

    #region ' PieceType Property '

    public static readonly DependencyProperty PieceTypeProperty =
      DependencyProperty.Register("PieceType", typeof (PieceType),
        typeof (HandNest), new UIPropertyMetadata(default(PieceType), OnPieceTypeChanged));

    private static void OnPieceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((HandNest) d).OnPieceTypeChanged((PieceType) e.NewValue);
    }

    private void OnPieceTypeChanged(PieceType pieceType)
    {
      if (Hand == null) return;
      PiecesCount = Count(Hand, pieceType);
      UpdateCp(pieceType, PieceColor);
    }

    public PieceType PieceType
    {
      get { return (PieceType) GetValue(PieceTypeProperty); }
      set { SetValue(PieceTypeProperty, value); }
    }

    #endregion

    #region ' PieceColor Property '

    public static readonly DependencyProperty PieceColorProperty =
      DependencyProperty.Register("PieceColor", typeof(PieceColor),
        typeof(HandNest), new UIPropertyMetadata(default(PieceColor), OnPieceColorChanged));

    private static void OnPieceColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((HandNest)d).OnPieceColorChanged((PieceColor)e.NewValue);
    }

    private void OnPieceColorChanged(PieceColor pieceColor)
    {
      UpdateCp(PieceType, pieceColor);
    }

    public PieceColor PieceColor
    {
      get { return (PieceColor)GetValue(PieceColorProperty); }
      set { SetValue(PieceColorProperty, value); }
    }
    


    #endregion
  }
}
