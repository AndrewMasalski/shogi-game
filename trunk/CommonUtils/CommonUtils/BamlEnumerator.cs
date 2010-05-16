using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;

namespace Yasc.Utils
{
  /// <summary>Class that enumerates all the baml streams in the input file</summary>
  public static class BamlEnumerator
  {
    const string Prefix = "/{0};component/";

    public static IEnumerable<T> ActivateObjectsOfType<T>(this Assembly assembly, string directory)
    {
      return GetBamlUris(assembly, directory).
        Select(uri => (T)Application.LoadComponent(uri));
    }

    public static IEnumerable<Uri> GetBamlUris(this Assembly assembly, string directory, string requestedResource = null)
    {
      string prefix = string.Format(Prefix, GetAsmName(assembly));
      return from path in GetBamlPaths(assembly, requestedResource)
             where string.Compare(Path.GetDirectoryName(path), directory, true, CultureInfo.InvariantCulture) == 0
             select new Uri(prefix + path, UriKind.Relative);
    }

    public static IEnumerable<Uri> GetBamlUris(this Assembly assembly, string requestedResource = null)
    {
      string prefix = string.Format(Prefix, GetAsmName(assembly));
      return from path in GetBamlPaths(assembly, requestedResource)
             select new Uri(prefix + path, UriKind.Relative);
    }

    /// <summary>Enumerates all the baml streams in the assembly</summary>
    /// <param name="assembly">assembly to enumerate bamls in</param>
    /// <param name="requestedResource">if null all resource fit</param>
    /// <returns>References to XAMLs</returns>
    public static IEnumerable<string> GetBamlPaths(this Assembly assembly, string requestedResource = null)
    {
      var manifestResourceStreams =
        from string resourceName in assembly.GetManifestResourceNames()
        select assembly.GetManifestResourceStream(resourceName);

      foreach (var stream in manifestResourceStreams)
        using (var reader = new ResourceReader(stream))
          foreach (var path in EnumerateBamlInResources(reader, requestedResource))
            yield return path;
    }

    private static string GetAsmName(Assembly asm)
    {
      string name = asm.FullName;
      if (name == null) throw new ArgumentOutOfRangeException("asm");
      return name.Substring(0, name.IndexOf(","));
    }

    /// <summary>Enumerate baml streams in a resources file</summary>        
    private static IEnumerable<string> EnumerateBamlInResources(ResourceReader resourceReader, string requestedResource)
    {
      return from DictionaryEntry resource in resourceReader 
             where IsResourceEntryBamlStream(resource, requestedResource) 
             select Path.ChangeExtension((string)resource.Key, ".xaml");
    }

    /// <summary>Determines whether a stream name and value pair indicates a baml stream</summary>
    private static bool IsResourceEntryBamlStream(DictionaryEntry resource, string requestedResource)
    {
      var resourceName = (string)resource.Key;
      string extension = Path.GetExtension(resourceName);

      if (requestedResource != null) // specific resource name requested
        if (!string.IsNullOrEmpty(resourceName)) // resource is named
          if (string.Compare(resourceName, requestedResource, true) != 0) // name fits
            return false;

      if (string.Compare(extension, ".Baml", true, CultureInfo.InvariantCulture) == 0)
      {
        if (typeof(Stream).IsAssignableFrom(resource.Value.GetType()))
          return true;
      }
      return false;
    }
  }
}