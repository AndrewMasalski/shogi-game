using System.IO;
using System.Reflection;
using System.Windows.Markup;

namespace Yasc.Utils.Skins
{
  internal static class BamlHelper
  {
    // Hope that Microsoft will not change this in the future, 
    // or at least provide an official way to load baml
    private static readonly MethodInfo _loadBamlMethod = typeof (XamlReader).
        GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static);

    public static TRoot LoadBaml<TRoot>(Stream stream)
    {
      var parserContext = new ParserContext();
      var parameters = new object[] {stream, parserContext, null, false};
      object bamlRoot = _loadBamlMethod.Invoke(null, parameters);
      return (TRoot) bamlRoot;
    }
  }
}