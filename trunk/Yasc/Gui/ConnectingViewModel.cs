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
    private Server _server;
    private readonly Dispatcher _dispatcher;
    private RelayCommand _cancelCommand;

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

    public ConnectingViewModel()
    {
      _dispatcher = Dispatcher.CurrentDispatcher;
    }

    public event EventHandler Fail;
    public event EventHandler Succeed;

    private void OnFail(EventArgs e)
    {
      var fail = Fail;
      if (fail != null) fail(this, e);
    }
    private void OnSucceed(EventArgs e)
    {
      var succeed = Succeed;
      if (succeed != null) succeed(this, e);
    }

    private void TryingToConnect(object state)
    {
      while (!string.IsNullOrEmpty(Address))
      {
        try
        {
          var server = Server.Connect(Address);
          server.Ping();
          _dispatcher.BeginInvoke(DispatcherPriority.Normal,
            new Action(() =>
                         {
                           Server = server;
                           LastError = null;
                         }));
          Thread.Sleep(1000);
          return;
        }
        catch (Exception x)
        {
          _dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            new Action(() => LastError = x));
        }
      }
    }

    public Exception LastError
    {
      get { return _lastError; }
      set
      {
        if (_lastError == value) return;
        _lastError = value;
        RaisePropertyChanged("LastError");
      }
    }

    private Exception _lastError;

  }
}
