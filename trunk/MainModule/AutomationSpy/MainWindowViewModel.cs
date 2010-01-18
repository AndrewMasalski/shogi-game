using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using AutomationSpy.Properties;
using MvvmFoundation.Wpf;

namespace AutomationSpy
{
  public class MainWindowViewModel : ObservableObject, IDisposable
  {
    private readonly Timer _timer;

    public AutomationElementViewModel[] Roots { get; private set; }

    public MainWindowViewModel()
    {
      Roots = new[]{ new AutomationElementViewModel(AutomationElement.RootElement, Condition.TrueCondition)};
      
      var rr = (int)Settings.Default.RefreshRate.TotalMilliseconds;
      _timer = new Timer(OnRefreshTimerTick, null, rr, rr);
    }

    #region ' Refresh On Timer '

    private void OnRefreshTimerTick(object state)
    {
      WalkExpandedElements(Roots, element => element.Refresh());
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

    #endregion

    public void Dispose()
    {
      _timer.Dispose();
    }
  }
}