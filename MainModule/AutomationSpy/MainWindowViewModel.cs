using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Threading;
using AutomationSpy.Properties;
using MvvmFoundation.Wpf;

namespace AutomationSpy
{
  public class MainWindowViewModel : ObservableObject
  {

    private void RefreshAll(object sender, EventArgs args)
    {
      WalkExpandedElements(Roots, RefreshElement);
    }

    private void RefreshElement(AutomationElementViewModel element)
    {
      if (element.IsSelected)
        foreach (var p in element.Properties)
          p.Refresh();

      if (!element.IsExpanded)
        foreach (var child in element.Children)
          child.ResetChildrenCache();

      element.RefreshChildren();
    }

    private static void WalkExpandedElements(
      IEnumerable<AutomationElementViewModel> list, 
      Action<AutomationElementViewModel> func)
    {
      foreach (var element in list)
      {
        func(element);
        if (element.IsExpanded)
        {
          WalkExpandedElements(element.Children, func);
        }
      }
    }

    public IEnumerable<AutomationElementViewModel> Roots { get; private set; }

    public MainWindowViewModel()
    {
      Roots = new AutomationElementViewModel(AutomationElement.RootElement).Children;

      var timer = new DispatcherTimer(DispatcherPriority.Background)
      {
        Interval = Settings.Default.RefreshRate,
        IsEnabled = true
      };
      timer.Tick += RefreshAll;
    }
  }
}