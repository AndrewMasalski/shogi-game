using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;

namespace Yasc.Skins
{
  internal static class BamlHelper
  {
    private static readonly MethodInfo LoadBamlMethod;

    static BamlHelper()
    {
      // Hope that Microsoft will not change this in the future, 
      // or at least provide an official way to load baml
      LoadBamlMethod = typeof (XamlReader).
        GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static);
    }

    public static TRoot LoadBaml<TRoot>(Stream stream)
    {
      var parserContext = new ParserContext();
      var parameters = new object[] {stream, parserContext, null, false};
      object bamlRoot = LoadBamlMethod.Invoke(null, parameters);
      return (TRoot) bamlRoot;
    }
  }
}