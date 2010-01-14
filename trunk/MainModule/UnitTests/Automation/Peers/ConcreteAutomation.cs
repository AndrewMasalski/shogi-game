using System;
using System.Windows.Automation;
using Yasc.Utils.Automation;

namespace UnitTests.Automation.Peers
{
  public abstract class ConcreteAutomation<T>
  {
    public AutomationElement Element { get; private set; }

    protected ConcreteAutomation(AutomationElement element)
    {
      if (element == null) throw new ArgumentNullException("element");
      string expectedType = typeof(T).Name;
      if (element.Current.ClassName != expectedType)
      {
        element = element.FindFirst(typeof (T));
        if (element == null)
          throw new ArgumentOutOfRangeException("element",
            "Given element is not of type " + expectedType + 
            " and doesn't contain automation tree children of that type");
      }

      Element = element;
    }
  }
}