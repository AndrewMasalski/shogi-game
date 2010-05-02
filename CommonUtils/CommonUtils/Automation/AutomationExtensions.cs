using System;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Controls;

namespace Yasc.Utils.Automation
{
  public static class AutomationExtensions
  {
    public static bool Trace { get; set; }

    private const int WiatCyclesCount = 30;
    private const int WaitCycleLength = 400;

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
    public static AutomationElement FindFirstByType(this AutomationElement element, Type type)
    {
      var result = element.FindFirstByFindFirstByTypeWait(type);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(WaitCycleLength);
        result = element.FindFirstByFindFirstByTypeWait(type);
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
    public static AutomationElement FindFirstByFindFirstByTypeWait(this AutomationElement element, Type type)
    {
      return element.FindFirst(TreeScope.Descendants, 
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name));
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
      if (Trace)
      {
        TraceIt(element, "--");
      }
      return element.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name));
    }

    private static void TraceIt(AutomationElement element, string prefix)
    {
      if (!string.IsNullOrWhiteSpace(element.Current.ClassName))
      {
        Console.WriteLine(prefix + element.Current.ClassName);
      }
      var children = element.FindAll(TreeScope.Children, Condition.TrueCondition);
      foreach (AutomationElement child in children)
      {
        TraceIt(child, prefix + "--");
      }
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
      var automationElement = element.FindFirstByName(name);
      if (automationElement == null)
        throw new ApplicationException(string.Format(
         "Couldn't have found element with name '{0}'", name));

      ((InvokePattern)automationElement.
         GetCurrentPattern(InvokePattern.Pattern)).Invoke();
    }
    public static void InvokeMenu(this AutomationElement element, string path)
    {
      var menu = element.FindFirstByType(typeof(Menu));
      if (menu == null) throw new ApplicationException("Menu not found");

      int counter = 0;
      var parts = path.Split('/');
      foreach (var name in parts)
      {
        menu = menu.FindFirstByName(name);
        if (menu == null) throw new ApplicationException("Menu not found");
        if (++counter < parts.Length)
        {
          menu.Pattern<ExpandCollapsePattern>().Expand();
        }
        else
        {
          menu.Pattern<InvokePattern>().Invoke();
        }
      }
    }

    public static T Pattern<T>(this AutomationElement element)
      where T : BasePattern
    {
      var pattern = (AutomationPattern)typeof(T).GetField("Pattern").GetValue(null);
      return (T)element.GetCurrentPattern(pattern);
    }
  }
}