using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Chess;
using System.Linq;

namespace Chess
{
  public class FontPiece : Control
  {
    private DispatcherTimer timer;
    #region ' ChessFont Property '

    public static readonly DependencyProperty ChessFontProperty =
      DependencyProperty.Register("ChessFont", typeof (ChessFont), typeof (FontPiece),
                                  new FrameworkPropertyMetadata(null, 
                                    FrameworkPropertyMetadataOptions.AffectsArrange, 
                                    OnChessFontChanged));

    private ContentPresenter _contentPresenter;
    private readonly TextBlock _textBlock;

    private static void OnChessFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((FontPiece) d).OnChessFontChanged((ChessFont) e.OldValue, (ChessFont) e.NewValue);
    }

    private void OnChessFontChanged(ChessFont oldValue, ChessFont newValue)
    {
      _textBlock.FontFamily = newValue.FontFamily;
      UpdateSymbol();
    }

    public ChessFont ChessFont
    {
      get { return (ChessFont) GetValue(ChessFontProperty); }
      set { SetValue(ChessFontProperty, value); }
    }

    #endregion

    #region ' Symbol Property '

    public static readonly DependencyProperty SymbolProperty =
      DependencyProperty.Register("Symbol", typeof (char), typeof (FontPiece),
                                  new PropertyMetadata(default(char), OnSymbolChanged));

    private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((FontPiece) d).OnSymbolChanged((char) e.OldValue, (char) e.NewValue);
    }

    private void OnSymbolChanged(char oldValue, char newValue)
    {
      UpdateSymbol();
    }

    public char Symbol
    {
      get { return (char) GetValue(SymbolProperty); }
      set { SetValue(SymbolProperty, value); }
    }

    private void UpdateSymbol()
    {
      if (ChessFont == null)
      {
        _textBlock.Text = "?";
        return;
      }
      _textBlock.Text = ChessFont.GetPiece(Symbol).ToString();
    }

    #endregion

    public FontPiece()
    {
      _textBlock = new TextBlock();
      timer =  new DispatcherTimer(TimeSpan.FromMilliseconds(100), 
        DispatcherPriority.Background, (s, e) => UpdateSymbol(), Dispatcher);
    }

    static FontPiece()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FontPiece),
        new FrameworkPropertyMetadata(typeof(FontPiece)));
    }

    public override void OnApplyTemplate()
    {
      _contentPresenter = this.FindVisualChildren<ContentPresenter>().FirstOrDefault();
      if (_contentPresenter != null)
      {
        _contentPresenter.Content = _textBlock;
      }
    }
    protected override Size MeasureOverride(Size constraint)
    {
      base.MeasureOverride(constraint);
      return new Size();
    }
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      if (ChessFont != null && _contentPresenter != null)
      {
        _textBlock.FontSize = ChessFont.FontSizeCoef*
          Math.Min(arrangeBounds.Width, arrangeBounds.Height);
      }
      return base.ArrangeOverride(arrangeBounds);
    }
  }
}
