using System;
using System.Collections.ObjectModel;
using MvvmFoundation.Wpf;
using Yasc.Skins;

namespace Yasc.Gui
{
  public class SkinningViewModel : ObservableObject
  {
    private SkinViewModel _selectedSkin;

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

    static SkinningViewModel()
    {
      Instance = new SkinningViewModel();
    }
    private SkinningViewModel()
    {
      DefaultSkin = new SkinViewModel(this, Skin.Null) {Name = "Default Skin"};

      AvailableSkins = new ObservableCollection<SkinViewModel> 
                         { 
                           DefaultSkin,
                           new SkinViewModel(this, new ReferencedAssemblySkin("Red", new Uri(@"/Yasc;component/Themes/Red.xaml", UriKind.Relative))), 
//                           new SkinViewModel(this, new DirectAssemblySkin("Sample Skin", @"x:\My SVN Projects\shogi-game\SampleSkin\bin\Debug\SampleSkin.dll")), 
                         };
      _selectedSkin = AvailableSkins[0];
    }
    public static SkinningViewModel Instance { get; private set; }
  }
}