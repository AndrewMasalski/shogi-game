using System.Collections.Generic;
using System.Windows.Automation;

namespace AutomationSpy
{
  public class MainWindowViewModel
  {
    public IEnumerable<AutomationElementViewModel> Roots { get; private set; }

    public MainWindowViewModel()
    {
      Roots = new AutomationElementViewModel(AutomationElement.RootElement).Children;
    }
  }
}