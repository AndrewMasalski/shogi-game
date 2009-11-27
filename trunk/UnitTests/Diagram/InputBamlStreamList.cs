using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace UnitTests.Diagram
{
  /// <summary>Class that enumerates all the baml streams in the input file</summary>
  public static class BamlEnumerator
  {
    const string Prefix = "/{0};component/";

    /// <summary>Enumerates all the baml streams in the assembly</summary>
    /// <param name="assembly">assembly to enumerate bamls in</param>
    /// <returns>References to XAMLs</returns>
    public static IEnumerable<Uri> GetBamls(this Assembly assembly)
    {
      string prefix = string.Format(Prefix, GetAsmName(assembly));

      var manifestResourceStreams =
        from string resourceName in assembly.GetManifestResourceNames()
        select assembly.GetManifestResourceStream(resourceName);

      foreach (var stream in manifestResourceStreams)
        using (var reader = new ResourceReader(stream))
          foreach (var uri in EnumerateBamlInResources(reader))
            yield return new Uri(prefix + uri, UriKind.Relative);
    }

    private static string GetAsmName(Assembly asm)
    {
      string name = asm.FullName;
      if (name == null) throw new ArgumentOutOfRangeException("asm");
      return name.Substring(0, name.IndexOf(","));
    }

    /// <summary>Enumerate baml streams in a resources file</summary>        
    private static IEnumerable<string> EnumerateBamlInResources(ResourceReader resourceReader)
    {
      foreach (DictionaryEntry resource in resourceReader)
        if (IsResourceEntryBamlStream(resource))
          yield return Path.ChangeExtension((string)resource.Key, ".xaml");
    }

    /// <summary>Determines whether a stream name and value pair indicates a baml stream</summary>
    private static bool IsResourceEntryBamlStream(DictionaryEntry resource)
    {
      string extension = Path.GetExtension((string)resource.Key);
      if (string.Compare(extension, ".Baml", true, CultureInfo.InvariantCulture) == 0)
      {
        if (typeof(Stream).IsAssignableFrom(resource.Value.GetType()))
          return true;
      }
      return false;
    }
  }
}