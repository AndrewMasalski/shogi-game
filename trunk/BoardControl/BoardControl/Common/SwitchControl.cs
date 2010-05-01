using System.Windows;
using System.Windows.Controls;

namespace Yasc.BoardControl.Common
{
  public class SwitchControl : ContentPresenter
  {
    static SwitchControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchControl),
        new FrameworkPropertyMetadata(typeof(SwitchControl)));
    }

    public static readonly DependencyProperty SwitcherProperty =
      DependencyProperty.Register("Switcher", typeof(object),
        typeof(SwitchControl), new UIPropertyMetadata(null, SwitcherPropertyChangedCallback));

    private static void SwitcherPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((SwitchControl)o).SwitcherPropertyChangedCallback(args.NewValue);
    }

    private void SwitcherPropertyChangedCallback(object value)
    {
      ContentTemplate = value == null ? null :
        TryFindResource(value.ToString()) as DataTemplate;
    }

    public object Switcher
    {
      get { return GetValue(SwitcherProperty); }
      set { SetValue(SwitcherProperty, value); }
    }

    protected override DataTemplate ChooseTemplate()
    {
      if (Switcher == null)
      {
        var template = TryFindResource("Default") as DataTemplate;
        if (template != null) return template;
      }
      return base.ChooseTemplate();
    }

    public SwitchControl()
    {
      Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
      ContentTemplate = Switcher == null ? null :
       TryFindResource(Switcher.ToString()) as DataTemplate;
    }
  }
}