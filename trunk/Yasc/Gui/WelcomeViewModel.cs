using System;
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

    public ICommand AutoplayCommand
    {
      get
      {
        if (_autoplayCommand == null)
        {
          _autoplayCommand = new RelayCommand(Autoplay);
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
          _connectCommand = new RelayCommand(Connect);
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
          _becomeServerCommand = new RelayCommand(BecomeServer);
        }
        return _becomeServerCommand;
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

    public WelcomeViewModel()
    {
      UserName = Settings.Default.UserName;
      SaveAndSkip = Settings.Default.SkipWelcomePage;
    }

    public event EventHandler ChoiceDone;

    private void Autoplay()
    {
      Mode = WelcomeChoice.Autoplay;
      Done();
    }
    private void Connect()
    {
      Mode = WelcomeChoice.ConnectToServer;
      Done();
    }
    private void BecomeServer()
    {
      Mode = WelcomeChoice.BecomeServer;
      Done();
    }
    private void Done()
    {
      Settings.Default.UserName = UserName;
      Settings.Default.SkipWelcomePage = SaveAndSkip;
      Settings.Default.DefaultStartMode = Mode;
      Settings.Default.Save();
      OnChoiceDone(EventArgs.Empty);
    }

    private void OnChoiceDone(EventArgs e)
    {
      var handler = ChoiceDone;
      if (handler != null) handler(this, e);
    }
  }
}
