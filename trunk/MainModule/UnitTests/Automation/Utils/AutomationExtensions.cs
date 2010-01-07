using System;
using System.Threading;
using System.Windows.Automation;

namespace UnitTests.Automation.Utils
{
  public static class AutomationExtensions
  {
    private const int WiatCyclesCount = 30;
    private const int WaitCycleLength = 40;

    public static AutomationElement FindFirstByAutomaionId(this AutomationElement element, string name)
    {
      var result = element.FindFirstByNameNoWait(name);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(WaitCycleLength);
        result = element.FindFirstByAutomaionIdNoWait(name);
      }
      return result;
    }
    public static AutomationElement FindFirstByName(this AutomationElement element, string name)
    {
      var result = element.FindFirstByNameNoWait(name);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(WaitCycleLength);
        result = element.FindFirstByNameNoWait(name);
      }
      return result;
    }
    public static AutomationElement FindFirstByName(this AutomationElement element, Type type, string name)
    {
      var result = element.FindFirstByNameNoWait(type, name);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(WaitCycleLength);
        result = element.FindFirstByNameNoWait(type, name);
      }
      return result;
    }
    public static AutomationElement FindFirstByAutomaionIdNoWait(this AutomationElement element, string name)
    {
      return element.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.AutomationIdProperty, name));
    }
    public static AutomationElement FindFirstByNameNoWait(this AutomationElement element, string name)
    {
      return element.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.NameProperty, name));
    }
    public static AutomationElement FindFirstByNameNoWait(this AutomationElement element, Type type, string name)
    {
      return element.FindFirst(TreeScope.Descendants, new AndCondition(
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name),
        new PropertyCondition(AutomationElement.NameProperty, name)));
    }

    public static AutomationElement FindFirst(this AutomationElement element, Type type)
    {
      var result = element.FindFirstNoWait(type);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(WaitCycleLength);
        result = element.FindFirstNoWait(type);
      }
      return result;
    }
    public static AutomationElement FindFirstNoWait(this AutomationElement element, Type type)
    {
      return element.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name));
    }

    public static AutomationElementCollection FindAll(this AutomationElement element, Type type, int expectedCount)
    {
      var result = element.FindAllNoWait(type);
      for (int i = 0; i < WiatCyclesCount && result.Count < expectedCount; i++)
      {
        Thread.Sleep(WaitCycleLength);
        result = element.FindAllNoWait(type);
      }
      return result;
    }
    public static AutomationElementCollection FindAllNoWait(this AutomationElement element, Type type)
    {
      return element.FindAll(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name));
    }

    public static void InvokeByName(this AutomationElement element, string name)
    {
      ((InvokePattern)element.FindFirstByName(name).
         GetCurrentPattern(InvokePattern.Pattern)).Invoke();
    }
    public static T Pattern<T>(this AutomationElement element)
      where T : BasePattern
    {
      var pattern = (AutomationPattern)typeof(T).GetField("Pattern").GetValue(null);
      return (T)element.GetCurrentPattern(pattern);
    }
  }
}