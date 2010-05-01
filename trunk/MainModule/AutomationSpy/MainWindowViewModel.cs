using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using AutomationSpy.Properties;
using Yasc.Utils.Mvvm;

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
      Roots[0].WalkExpandedElements(element => element.Refresh());
    }

    #endregion

    public void Dispose()
    {
      _timer.Dispose();
    }
  }
}