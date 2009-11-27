using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace UnitTests
{
  /// <summary>Class that enumerates all the baml streams in the input file</summary>
  public static class BamlEnumerator
  {
    const string Prefix = "/{0};component/";

    /// <summary>Enumerates all the baml streams in the input assembly</summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IEnumerable<Uri> GetBamls(this Assembly assembly)
    {
      string prefix = string.Format(Prefix, GetAsmName(assembly));

      foreach (string resourceName in assembly.GetManifestResourceNames())
        if (assembly.GetManifestResourceStream(resourceName) != null)
// ReSharper disable AssignNullToNotNullAttribute
          using (var reader = new ResourceReader(assembly.GetManifestResourceStream(resourceName)))
// ReSharper restore AssignNullToNotNullAttribute
            foreach (var uri in EnumerateBamlInResources(reader, prefix))
              yield return uri;
    }

    private static string GetAsmName(Assembly asm)
    {
      string name = asm.FullName;
// ReSharper disable PossibleNullReferenceException
      return name.Substring(0, name.IndexOf(","));
// ReSharper restore PossibleNullReferenceException
    }

    /// <summary>Enumerate baml streams in a resources file</summary>        
    private static IEnumerable<Uri> EnumerateBamlInResources(ResourceReader resourceReader, string prefix)
    {
      foreach (DictionaryEntry resource in resourceReader)
        if (IsResourceEntryBamlStream(resource))
          yield return new Uri(prefix +
            Path.ChangeExtension((string)resource.Key, ".xaml"), UriKind.Relative);
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