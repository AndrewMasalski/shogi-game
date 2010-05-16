using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Yasc.Utils.Skins
{
  /// <summary>Loads skin from specified assembly in the same domain. 
  /// Works if assembly wasn't referenced at compile time.</summary>
  internal class DirectAssemblySkin : Skin
  {
    private string _resourceName;

    #region ' Ctor '

    public DirectAssemblySkin(string name, string assemblyPath)
      : base(name)
    {
      if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentException("Invalid assembly path", "assemblyPath");
      AssemblyName = AssemblyName.GetAssemblyName(assemblyPath);
    }
    public DirectAssemblySkin(string name, AssemblyName assemblyName)
      : base(name)
    {
      if (assemblyName == null) throw new ArgumentNullException("assemblyName");
      AssemblyName = assemblyName;
    }
    public DirectAssemblySkin(string name, string assemblyPath, string resourceName)
      : base(name)
    {
      if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentException("Invalid assembly path", "assemblyPath");
      if (string.IsNullOrEmpty(resourceName)) throw new ArgumentException("Invalid resource name", "resourceName");

      AssemblyName = AssemblyName.GetAssemblyName(assemblyPath);
      _resourceName = resourceName;
      FixResourceName();
    }
    public DirectAssemblySkin(string name, AssemblyName assemblyName, string resourceName)
      : base(name)
    {
      if (assemblyName == null) throw new ArgumentNullException("assemblyName");
      if (string.IsNullOrEmpty(resourceName)) throw new ArgumentException("Invalid resource name", "resourceName");

      AssemblyName = assemblyName;
      _resourceName = resourceName;
      FixResourceName();
    }

    #endregion

    protected AssemblyName AssemblyName { get; private set; }
    protected override sealed void LoadResources()
    {
      Resources.AddRange(
        from uri in Assembly.Load(AssemblyName).GetBamlUris(_resourceName)
        let skin = (ResourceDictionary)Application.LoadComponent(uri)
        where skin != null
        select skin);
    }

    private void FixResourceName()
    {
      _resourceName = _resourceName.ToLower().Replace(".xaml", ".baml");
    }
  }
}