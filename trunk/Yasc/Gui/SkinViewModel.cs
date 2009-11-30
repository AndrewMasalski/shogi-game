using System;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.Skins;

namespace Yasc.Gui
{
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
      get
      {
        if (_loadCommand == null)
        {
          _loadCommand = new RelayCommand(() => IsSelected = true);
        }
        return _loadCommand;
      }
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