using System;
using System.IO;

namespace UnitTests
{
  public static class Utils
  {
    public static string Relative(string relativePath)
    {
      var s = Environment.CurrentDirectory;
      s = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(s)));
      s = Path.Combine(s, "Engines");
      return Path.Combine(s, relativePath);
    }
    
  }
}