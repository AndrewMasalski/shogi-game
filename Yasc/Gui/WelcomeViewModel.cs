using System;
using System.Windows.Input;
using MvvmFoundation.Wpf;

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
      OnChoiceDone(EventArgs.Empty);
    }

    
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
      ConnectingViewModel = new ConnectingViewModel();
    }

    public ConnectingViewModel ConnectingViewModel { get; private set; }
  }
}
