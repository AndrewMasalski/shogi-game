using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Yasc.Utils.Skins
{
  public interface ISkinBamlResolver
  {
    List<Stream> GetSkinBamlStreams(AssemblyName skinAssemblyName);
    List<Stream> GetSkinBamlStreams(AssemblyName skinAssemblyName, string resourceName);
  }
}