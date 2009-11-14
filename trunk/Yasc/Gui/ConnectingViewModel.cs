using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using MvvmFoundation.Wpf;
using Yasc.Networking;

namespace Yasc.Gui
{
  public class ConnectingViewModel : ObservableObject
  {
    private string _address;
    public string Address
    {
      get { return _address; }
      set
      {
        if (_address == value) return;
        bool wasEmpty = string.IsNullOrEmpty(_address);
        _address = value;
        RaisePropertyChanged("Address");
        if (wasEmpty && !string.IsNullOrEmpty(_address))
          ThreadPool.QueueUserWorkItem(TryingToConnect);
      }
    }

    public ICommand CancelCommand
    {
      get
      {
        if (_cancelCommand == null)
        {
          _cancelCommand = new RelayCommand(() => OnFail(EventArgs.Empty));
        }
        return _cancelCommand;
      }
    }

    public ConnectingViewModel()
    {
      _dispatcher = Dispatcher.CurrentDispatcher;
    }

    private void TryingToConnect(object state)
    {
      while (!string.IsNullOrEmpty(Address))
      {
        try
        {
          var server = Server.Connect(Address);
          server.Ping();
          _dispatcher.BeginInvoke(
            DispatcherPriority.Normal, 
            new Action(() => Server = server));
          return;
        }
        catch (Exception x)
        {
          Console.WriteLine(x);
        }
      }
    }

    private Server _server;
    private readonly Dispatcher _dispatcher;
    private RelayCommand _cancelCommand;

    public Server Server
    {
      get { return _server; }
      private set
      {
        if (_server == value) return;
        _server = value;
        RaisePropertyChanged("Server");
        OnSucceed(EventArgs.Empty);
      }
    }

    public event EventHandler Fail;

    private void OnFail(EventArgs e)
    {
      var fail = Fail;
      if (fail != null) fail(this, e);
    }

    public event EventHandler Succeed;

    private void OnSucceed(EventArgs e)
    {
      var succeed = Succeed;
      if (succeed != null) succeed(this, e);
    }
  }
}
