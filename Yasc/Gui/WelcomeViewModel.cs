using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.Properties;

namespace Yasc.Gui
{
  public class WelcomeViewModel : ObservableObject
  {
    private string _userName;
    private bool _saveAndSkip;
    private string _address;
    private WelcomeChoice _mode;
    private RelayCommand _autoplayCommand;
    private RelayCommand _connectCommand;
    private RelayCommand _becomeServerCommand;
    private RelayCommand _playWithCompCommand;

    public ICommand AutoplayCommand
    {
      get
      {
        if (_autoplayCommand == null)
        {
          _autoplayCommand = new RelayCommand(
            () => Done(WelcomeChoice.Autoplay));
        }
        return _autoplayCommand;
      }
    }
    public ICommand ConnectCommand
    {
      get
      {
        if (_connectCommand == null)
        {
          _connectCommand = new RelayCommand(
            () => Done(WelcomeChoice.ConnectToServer));
        }
        return _connectCommand;
      }
    }
    public ICommand BecomeServerCommand
    {
      get
      {
        if (_becomeServerCommand == null)
        {
          _becomeServerCommand = new RelayCommand(
            () => Done(WelcomeChoice.BecomeServer));
        }
        return _becomeServerCommand;
      }
    }
    public ICommand PlayWithCompCommand
    {
      get
      {
        if (_playWithCompCommand == null)
        {
          _playWithCompCommand = new RelayCommand(
            () => Done(WelcomeChoice.ArtificialIntelligence));
        }
        return _playWithCompCommand;
      }
    }

    public string UserName
    {
      get { return _userName; }
      set
      {
        if (_userName == value) return;
        _userName = value;
        RaisePropertyChanged("UserName");
      }
    }
    public string Address
    {
      get { return _address; }
      set
      {
        if (_address == value) return;
        _address = value;
        RaisePropertyChanged("Address");
      }
    }
    public WelcomeChoice Mode
    {
      get { return _mode; }
      set
      {
        if (_mode == value) return;
        _mode = value;
        RaisePropertyChanged("Mode");
      }
    }
    public bool SaveAndSkip
    {
      get { return _saveAndSkip; }
      set
      {
        if (_saveAndSkip == value) return;
        _saveAndSkip = value;
        RaisePropertyChanged("SaveAndSkip");
      }
    }
    public ObservableCollection<string> LastVsistedServers { get; private set; }
    public WelcomeViewModel()
    {
      Address = Settings.Default.Address;
      UserName = Settings.Default.UserName;
      SaveAndSkip = Settings.Default.SkipWelcomePage;
      LastVsistedServers = new ObservableCollection<string>(Settings.Default.LoadLvs());

      if (string.IsNullOrEmpty(Address)) Address = "localhost";
      if (string.IsNullOrEmpty(UserName)) UserName = "John Doe";
    }

    public event EventHandler ChoiceDone;

    private void Done(WelcomeChoice mode)
    {
      Mode = mode;

      var s = Settings.Default;
      s.UserName = UserName;
      s.SkipWelcomePage = SaveAndSkip;
      s.DefaultStartMode = Mode;
      s.Address = Address;
      s.SaveLvs(LastVsistedServers, Address);

      s.Save();
      OnChoiceDone(EventArgs.Empty);
    }
    private void OnChoiceDone(EventArgs e)
    {
      var handler = ChoiceDone;
      if (handler != null) handler(this, e);
    }
  }
}
