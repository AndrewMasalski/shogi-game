using System.Windows.Automation;
using Yasc.Utils.Mvvm;

namespace AutomationSpy
{
  public class AutomationPatternViewModel : ObservableObject
  {
    public string Caption
    {
      get { return _caption; }
      set
      {
        if (_caption == value) return;
        _caption = value;
        RaisePropertyChanged("Caption");
      }
    }

    private string _caption;

    public AutomationPatternViewModel(AutomationElement element, AutomationPattern pattern)
    {
      _caption = pattern.ProgrammaticName.Replace("PatternIdentifiers.Pattern", "");
    }
  }
}