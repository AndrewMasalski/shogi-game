using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Yasc.Utils.Skins
{
  public abstract class Skin
  {
    private readonly string _name;
    private readonly List<ResourceDictionary> 
      _resources = new List<ResourceDictionary>();

    protected Skin(string name)
    {
      _name = name;
    }
    protected List<ResourceDictionary> Resources
    {
      get { return _resources; }
    }
    protected abstract void LoadResources();

    public string Name
    {
      get { return _name; }
    }
    public virtual void Load()
    {
      if (Resources.Count != 0)
      {
        // Already loaded
        return;
      }
      LoadResources();
      foreach (ResourceDictionary skinResource in Resources)
        Application.Current.Resources.MergedDictionaries.Add(skinResource);
    }
    public virtual void Unload()
    {
      foreach (ResourceDictionary skinResource in Resources)
        Application.Current.Resources.MergedDictionaries.Remove(skinResource);
      Resources.Clear();
    }

    #region ' Nested type: NullSkin '

    public static readonly Skin Null = new NullSkin();

    private sealed class NullSkin : Skin
    {
      public NullSkin()
        : base("Null skin")
      {
      }

      protected override void LoadResources()
      {
      }
    }

    #endregion

    public static Skin Load(string name, string assemblyPath)
    {
      return new DirectAssemblySkin(name, assemblyPath);
    }
    public static Skin Load(string name, AssemblyName assemblyName)
    {
      return new DirectAssemblySkin(name, assemblyName);
    }
    public static Skin Load(string name, string assemblyPath, string resourceName)
    {
      return new DirectAssemblySkin(name, assemblyPath, resourceName);
    }
    public static Skin Load(string name, AssemblyName assemblyName, string resourceName)
    {
      return new DirectAssemblySkin(name, assemblyName, resourceName);
    }
    public static Skin Load(string name, Uri resourceUri) 
    {
      return new ReferencedAssemblySkin(name, resourceUri);
    }  
    public static Skin LoadXaml(string name, Uri resourceUri) 
    {
      return new LooseXamlSkin(name, resourceUri);
    }
    public static Skin LoadXaml(string name, IEnumerable<Uri> resourceUri) 
    {
      return new LooseXamlSkin(name, resourceUri);
    }  
  }
}