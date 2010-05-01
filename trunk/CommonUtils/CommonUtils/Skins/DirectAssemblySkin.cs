using System;
using System.Reflection;
using System.Windows;

namespace Yasc.Utils.Skins
{
  public class DirectAssemblySkin : Skin
  {
    private readonly AssemblyName _fullName;
    private string _resourceName;

    public DirectAssemblySkin(string name, string assemblyPath) 
      : base(name)
    {
      if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentException("Invalid assembly path", "assemblyPath");
      _fullName = AssemblyName.GetAssemblyName(assemblyPath);
    }
    public DirectAssemblySkin(string name, AssemblyName fullName)
      : base(name)
    {
      if (fullName == null) throw new ArgumentNullException("fullName");
      _fullName = fullName;
    }
    public DirectAssemblySkin(string name, string assemblyPath, string resourceName)
      : base(name)
    {
      if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentException("Invalid assembly path", "assemblyPath");
      if (string.IsNullOrEmpty(resourceName)) throw new ArgumentException("Invalid resource name", "resourceName");

      _fullName = AssemblyName.GetAssemblyName(assemblyPath);
      _resourceName = resourceName;
      FixResourceName();
    }
    public DirectAssemblySkin(string name, AssemblyName fullName, string resourceName)
      : base(name)
    {
      if (fullName == null) throw new ArgumentNullException("fullName");
      if (string.IsNullOrEmpty(resourceName)) throw new ArgumentException("Invalid resource name", "resourceName");

      _fullName = fullName;
      _resourceName = resourceName;
      FixResourceName();
    }

    protected AssemblyName FullName
    {
      get { return _fullName; }
    }
    protected override sealed void LoadResources()
    {
      var skinResolver = PreLoadResources();
      try
      {
        var skinBamlStreams = skinResolver.
          GetSkinBamlStreams(_fullName, _resourceName);

        foreach (var resourceStream in skinBamlStreams)
        {
          var skinResource = BamlHelper.LoadBaml<ResourceDictionary>(resourceStream);
          if (skinResource != null)
          {
            Resources.Add(skinResource);
          }
        }
      }
      finally
      {
        PostLoadResources();
      }
    }

    protected virtual ISkinBamlResolver PreLoadResources()
    {
      return new SkinBamlResolver();
    }
    protected virtual void PostLoadResources()
    {
    }

    private void FixResourceName()
    {
      _resourceName = _resourceName.ToLower().Replace(".xaml", ".baml");
    }
  }
}