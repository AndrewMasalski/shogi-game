using System;
using System.Windows.Input;
using Yasc.Utils.Mvvm;
using Yasc.Utils.Skins;

namespace MainModule.ViewModel
{
  /// <summary>Represents a skin</summary>
  public class SkinViewModel : ObservableObject
  {
    private readonly SkinningViewModel _owner;
    private ICommand _loadCommand;

    public Skin Skin { get; private set; }
    public string Name { get; set; }
    public bool IsSelected
    {
      get { return _owner.SelectedSkin == this; }
      set
      {
        if (IsSelected == value) return;
        _owner.SelectedSkin = value ? this : _owner.DefaultSkin;
      }
    }
    public ICommand LoadCommand
    {
      get { return _loadCommand ?? (_loadCommand = new RelayCommand(() => IsSelected = true)); }
    }

    public SkinViewModel(SkinningViewModel owner, Skin skin)
    {
      if (owner == null) throw new ArgumentNullException("owner");
      if (skin == null) throw new ArgumentNullException("skin");

      _owner = owner;
      Skin = skin;
      Name = skin.Name;
    }
    
    internal void RaiseIsSelectedChanged()
    {
      RaisePropertyChanged("IsSelected");
    }
  }
}