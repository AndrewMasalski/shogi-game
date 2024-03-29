using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Yasc.BoardControl.Controls;
using Yasc.Utils;
using Yasc.Utils.Mvvm;
using Yasc.Utils.Skins;

namespace MainModule.ViewModel
{
  /// <summary>Represents a set of skins</summary>
  public class SkinningViewModel : ObservableObject
  {
    #region ' Fields '

    private SkinViewModel _selectedSkin;

    #endregion

    #region ' Public Properties '

    public SkinViewModel SelectedSkin
    {
      get { return _selectedSkin; }
      set
      {
        if (_selectedSkin == value) return;
        if (!AvailableSkins.Contains(value)) throw new ArgumentOutOfRangeException("value");

        var old = _selectedSkin;
        _selectedSkin = value;

        old.Skin.Unload();
        old.RaiseIsSelectedChanged();

        _selectedSkin.Skin.Load();
        _selectedSkin.RaiseIsSelectedChanged();
      }
    }
    public SkinViewModel DefaultSkin { get; private set; }
    public ObservableCollection<SkinViewModel> AvailableSkins { get; private set; }

    #endregion

    #region ' Implementation '

    private SkinningViewModel()
    {
      DefaultSkin = new SkinViewModel(this, Skin.Null) { Name = "Default Skin" };

      AvailableSkins = new ObservableCollection<SkinViewModel>(
        new[] {DefaultSkin}.
          Concat(GetLocalSkins()).
          Concat(GetDirectAssemblySkins()));

      _selectedSkin = AvailableSkins[0];
    }
    private IEnumerable<SkinViewModel> GetLocalSkins()
    {
      const string generic = @"/Yasc.BoardControl;component/Themes/Generic.xaml";
      foreach (var uri in typeof(ShogiBoard).Assembly.GetBamlUris("Themes", null))
        if (string.Compare(uri.OriginalString, generic, true) != 0)
          yield return new SkinViewModel(this, 
            Skin.Load(uri.ToString(), uri));
    }
    private IEnumerable<SkinViewModel> GetDirectAssemblySkins()
    {
      string asssemplyLocation = typeof(SkinViewModel).Assembly.Location;
      string baseDirectory = Path.GetDirectoryName(asssemplyLocation);
      baseDirectory = Path.Combine(baseDirectory, "Skins");
      if (!Directory.Exists(baseDirectory)) yield break;
      foreach (var skin in Directory.GetFiles(baseDirectory, "*.dll"))
      {
        string skinName = Path.GetFileName(skin);
        var assemblySkin = Skin.Load(skinName, skin);
        yield return new SkinViewModel(this, assemblySkin);
      }
    }

    #endregion

    #region ' Statics '

    static SkinningViewModel()
    {
      Instance = new SkinningViewModel();
    }
    public static SkinningViewModel Instance { get; private set; }

    #endregion
  }
}