using System;
using System.Collections.Generic;
using System.Windows;

namespace Yasc.Skins
{
  internal sealed class LooseXamlSkin : Skin
  {
    private readonly List<Uri> _sources;

    public LooseXamlSkin(string name, Uri source) 
      : base(name)
    {
      _sources = new List<Uri> { source };
    }

    public LooseXamlSkin(string name, IEnumerable<Uri> sources) 
      : base(name)
    {
      _sources = new List<Uri>(sources);
    }

    protected override void LoadResources()
    {
      foreach (var uri in _sources)
        Resources.Add(new ResourceDictionary {Source = uri});
    }
  }
}