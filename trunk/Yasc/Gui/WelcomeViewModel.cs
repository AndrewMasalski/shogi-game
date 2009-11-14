using System;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.Properties;

namespace Yasc.Gui
{
  public class WelcomeViewModel : ObservableObject
  {
    private WelcomeChoice _mode;
    public event EventHandler ChoiceDone;

    private RelayCommand _autoplayCommand;

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

    private void Autoplay()
    {
      Mode = WelcomeChoice.Autoplay;
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

    private string _userName;
    

    private void OnChoiceDone(EventArgs e)
    {
      var handler = ChoiceDone;
      if (handler != null) handler(this, e);
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

    public WelcomeViewModel()
    {
      UserName = Settings.Default.UserName;
      SaveAndSkip = Settings.Default.SkipWelcomePage;
      ConnectingViewModel = new ConnectingViewModel();
    }

    public ConnectingViewModel ConnectingViewModel { get; private set; }

    private bool _saveAndSkip;
    
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
  }
}
