using System;
using System.Reflection;

namespace Yasc.Skins
{
  public sealed class AppDomainAssemblySkin : DirectAssemblySkin
  {
    private AppDomain _assemblySkinDomain;

    public AppDomainAssemblySkin(string name, string assemblyPath) 
      : base(name, assemblyPath)
    {
    }

    public AppDomainAssemblySkin(string name, AssemblyName fullName)
      : base(name, fullName)
    {
    }

    public AppDomainAssemblySkin(string name, string assemblyPath, string resourceName)
      : base(name, assemblyPath, resourceName)
    {
    }

    public AppDomainAssemblySkin(string name, AssemblyName fullName, string resourceName)
      : base(name, fullName, resourceName)
    {
    }

    protected override ISkinBamlResolver PreLoadResources()
    {
      _assemblySkinDomain = AppDomain.CreateDomain(Name);
      var skinResolver = (ISkinBamlResolver) 
                         _assemblySkinDomain.CreateInstanceAndUnwrap(
                           Assembly.GetExecutingAssembly().FullName,
                           typeof (SkinBamlResolver).FullName);
      return skinResolver;
    }

    protected override void PostLoadResources()
    {
      if (_assemblySkinDomain == null) return;
      AppDomain.Unload(_assemblySkinDomain);
      _assemblySkinDomain = null;
    }
  }
}