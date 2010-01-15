using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;

namespace AutomationSpy
{
  public class AutomationElementViewModel
  {
    public AutomationElement Element { get; private set; }

    public AutomationElementViewModel(AutomationElement element)
    {
      Element = element;
    }

    public string Caption
    {
      get 
      { 
        return !string.IsNullOrEmpty(Element.Current.Name) ?
          Element.Current.Name : Element.Current.ClassName;
      }
    }

    public IEnumerable<ElementPropertyViewModel> Properties
    {
      get
      {
        return from p in Element.GetSupportedProperties()
               select new ElementPropertyViewModel(Element, p);
      }
    }

    public IEnumerable<AutomationElementViewModel> Children
    {
      get
      {
        return from r in Element.FindAll(TreeScope.Children, Condition.TrueCondition).
                         Cast<AutomationElement>()
               select new AutomationElementViewModel(r);
      }
    }
  }
}