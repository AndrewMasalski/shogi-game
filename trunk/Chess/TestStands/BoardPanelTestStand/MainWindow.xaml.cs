using System;
using System.Collections;
using System.Linq;

namespace BoardPanelTestStand
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      _fonts.ItemsSource = GetResources()
        .Where(r => r.OriginalString.Split('/').Length == 3)
        .Select(r => r.OriginalString.Split('/')[1])
        .Distinct()
        .Select(folder => new PieceSet(folder))
        .ToList();
    }
    public static Uri[] GetResources()
    {
      var asm = typeof (ColoredChessFonts.App).Assembly;
      var resName = asm.GetName().Name + ".g.resources";
      using (var stream = asm.GetManifestResourceStream(resName))
      using (var reader = new System.Resources.ResourceReader(stream))
        return reader.Cast<DictionaryEntry>()
          .Select(entry => ((string)entry.Key))
          .Where(key => key.Contains(".baml"))
          .Select(key => key.Replace(".baml", ".xaml"))
          .Select(key => new Uri(key, UriKind.Relative))
          .ToArray();
    }
  }
  public class PieceSet
  {
    public Uri Uri { get; private set; }
    public string Name { get; private set; }
    public PieceSet(string folder)
    {
      Name = folder.Replace("%20", " ");
      Uri = new Uri("/ColoredChessFonts;component/PieceSets/" + folder, UriKind.Relative);
    }
    public override string ToString()
    {
      return Name;
    }
  }
}
