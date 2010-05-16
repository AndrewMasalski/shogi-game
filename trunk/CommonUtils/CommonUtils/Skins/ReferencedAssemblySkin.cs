using System;
using System.Windows;

namespace Yasc.Utils.Skins
{
  /// <summary>Loads skin by Uri. Works if assembly was referenced at compile time</summary>
  internal sealed class ReferencedAssemblySkin : Skin
  {
    private readonly Uri _resourceUri;

    public ReferencedAssemblySkin(string name, Uri resourceUri) 
      : base(name)
    {
      _resourceUri = resourceUri;
    }

    protected override void LoadResources()
    {
      Resources.Add((ResourceDictionary) Application.LoadComponent(_resourceUri));
    }
  }
}