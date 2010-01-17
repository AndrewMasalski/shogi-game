using System;
using System.Collections;
using System.Windows.Automation;
using System.Linq;
using MvvmFoundation.Wpf;

namespace AutomationSpy
{
  public class ElementPropertyViewModel : ObservableObject
  {
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
      Value = GetValue();
    }

    public string Name
    {
      get { return Property.ProgrammaticName.
        Replace("AutomationElementIdentifiers.", "").
        Replace("Property", ""); }
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

    private string _value;

    private string GetValue()
    {
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
}