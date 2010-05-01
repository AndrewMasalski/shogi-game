using System.Collections;
using System.Linq;
using System.Windows.Automation;
using Yasc.Utils.Mvvm;

namespace AutomationSpy
{
  public class ElementPropertyViewModel : ObservableObject
  {
    private string _value;

    public string Name
    {
      get
      {
        return Property.ProgrammaticName.
          Replace("AutomationElementIdentifiers.", "").
          Replace("Property", "");
      }
    }
    public string Value
    {
      get { return _value; }
      set
      {
        if (_value == value) return;
        _value = value;
        RaisePropertyChanged("Value");
      }
    }
    public AutomationElement Element { get; set; }
    public AutomationProperty Property { get; set; }

    public ElementPropertyViewModel(AutomationElement element, AutomationProperty property)
    {
      Element = element;
      Property = property;
      Value = GetValue();
    }

    public void Refresh()
    {
      // This is called on timer thread!
      Value = GetValue();
    }
    private string GetValue()
    {
      // This is called on timer thread!
      var value = Element.GetCurrentPropertyValue(Property);
      var str = value as string;
      if (str != null)
      {
        return str == "" ? "<empty>" : str;
      }
      var list = value as IEnumerable;
      if (list != null)
      {
        return "(" + string.Join(", ", list.Cast<object>().Select(o => o.ToString()).ToArray()) + ")";
      }
      return value != null ? value.ToString() : "<null>";
    }
  }

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