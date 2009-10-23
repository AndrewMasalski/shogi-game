using System;
using System.Windows;
using System.Windows.Media;
using Yasc.GenericDragDrop;

namespace Yasc
{
  public partial class DragDropAdorner
  {
    public DragDropAdorner()
    {
      InitializeComponent();
      DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
     // throw new NotImplementedException();
    }

    protected override void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
//      if (a.Visual != null)
//      a.Visual.ToString();
      switch ((DropState)e.NewValue)
      {
        case DropState.CanDrop:
//          _back.Stroke = Application.Current.Resources["canDropBrush"] as SolidColorBrush;
//          _indicator.Source = Application.Current.Resources["dropIcon"] as DrawingImage;
          break;
        case DropState.CannotDrop:
//          _back.Stroke = Application.Current.Resources["solidRed"] as SolidColorBrush;
//          _indicator.Source = Application.Current.Resources["noDropIcon"] as DrawingImage;
          break;
      }
    }
  }
}