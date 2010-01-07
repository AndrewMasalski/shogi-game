using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Yasc.Skins
{
  internal class SkinBamlResolver : MarshalByRefObject, ISkinBamlResolver
  {
    public List<Stream> GetSkinBamlStreams(AssemblyName skinAssemblyName)
    {
      return GetSkinBamlStreams(skinAssemblyName, string.Empty);
    }
    public List<Stream> GetSkinBamlStreams(AssemblyName skinAssemblyName, string bamlResourceName)
    {
      var skinBamlStreams = new List<Stream>();
      Assembly skinAssembly = Assembly.Load(skinAssemblyName);
      string[] resourcesNames = skinAssembly.GetManifestResourceNames();
      foreach (string resourceName in resourcesNames)
      {
        ManifestResourceInfo resourceInfo = skinAssembly.GetManifestResourceInfo(resourceName);
        if (resourceInfo != null && resourceInfo.ResourceLocation == ResourceLocation.ContainedInAnotherAssembly)
          continue;

        Stream resourceStream = skinAssembly.GetManifestResourceStream(resourceName);
        using (var resourceReader = new ResourceReader(resourceStream))
        {
          foreach (DictionaryEntry entry in resourceReader)
          {
            if (IsRelevantResource(entry, bamlResourceName))
            {
              skinBamlStreams.Add(entry.Value as Stream);
            }
          }
        }
      }
      return skinBamlStreams;
    }

    private static bool IsRelevantResource(DictionaryEntry entry, string resourceName)
    {
      var entryName = entry.Key as string;
      string extension = Path.GetExtension(entryName);
      return
        string.Compare(extension, ".baml", true) == 0 && // the resource has a .baml extension
        entry.Value is Stream && // the resource is a Stream
        (string.IsNullOrEmpty(resourceName) || string.Compare(resourceName, entryName, true) == 0);
      // the resource name requested equals to current resource name
    }
  }
}