using System;

namespace BoardControl.AutomationTests.Peers
{
  public static class Uia
  {
    public static string UiaClassName(this Type systemType)
    {
      return "Uia." + systemType.Name;
    }
  }
}