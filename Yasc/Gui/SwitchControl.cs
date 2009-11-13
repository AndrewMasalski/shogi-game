using System.Windows;
using System.Windows.Controls;

namespace Yasc.Gui
{
  public class SwitchControl : ContentPresenter
  {
    static SwitchControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchControl),
        new FrameworkPropertyMetadata(typeof(SwitchControl)));
    }

    public SwitchControl()
    {
      ContentTemplateSelector = new MyTemplateSelector();
    }

    private class MyTemplateSelector : DataTemplateSelector
    {
      public override DataTemplate SelectTemplate(object item, DependencyObject container)
      {
        if (item == null) return null;
        return ((FrameworkElement)container).
          TryFindResource(item.ToString()) as DataTemplate;
      }
    }
  }
}