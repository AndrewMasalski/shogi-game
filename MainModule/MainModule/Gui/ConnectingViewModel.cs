using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Yasc.Networking;
using Yasc.Networking.Interfaces;
using Yasc.Utils.Mvvm;

namespace MainModule.Gui
{
  public class ConnectingViewModel : ObservableObject
  {
    private readonly string _userName;
    private ShogiServer _server;
    private bool _isConnecting;
    private Exception _lastError;
    private readonly Dispatcher _dispatcher;
    private RelayCommand _cancelCommand;
    private RelayCommand _retryCommand;
    private IServerSession _session;

    public ICommand CancelCommand
    {
      get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(() => OnFail(EventArgs.Empty))); }
    }
    public ICommand RetryCommand
    {
      get { return _retryCommand ?? (_retryCommand = new RelayCommand(Retry)); }
    }

    public string Address { get; private set; }

    public bool IsConnecting
    {
      get { return _isConnecting; }
      set
      {
        if (_isConnecting == value) return;
        _isConnecting = value;
        RaisePropertyChanged("IsConnecting");
      }
    }
    public ShogiServer Server
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

    public IServerSession Session
    {
      get { return _session ?? (_session = Server.LogOn(_userName)); }
    }

    public ConnectingViewModel(string address, string userName)
    {
      if (string.IsNullOrEmpty(address)) throw new ArgumentNullException("address");
      if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");
      Address = address;
      _userName = userName;
      _dispatcher = Dispatcher.CurrentDispatcher;
      ThreadPool.QueueUserWorkItem(TryingToConnect);
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

    private void Retry()
    {
      if (!IsConnecting)
        ThreadPool.QueueUserWorkItem(TryingToConnect);
    }

    private void TryingToConnect(object state)
    {
      try
      {
        AsynchConnectionStarted();
        var server = ShogiServer.Connect(Address);
        server.Ping();
        AsynchConnectionSucceed(server);
      }
      catch (Exception x)
      {
        AsynchConnectionFailed(x);
      }
    }

    private void AsynchConnectionStarted()
    {
      _dispatcher.BeginInvoke(DispatcherPriority.Normal,
                              new Action(() =>
                              {
                                IsConnecting = true;
                                LastError = null;
                              }));
    }
    private void AsynchConnectionSucceed(ShogiServer server)
    {
      _dispatcher.BeginInvoke(DispatcherPriority.Normal,
                              new Action(() =>
                              {
                                Server = server;
                                LastError = null;
                                IsConnecting = false;
                              }));
    }
    private void AsynchConnectionFailed(Exception x)
    {
      _dispatcher.BeginInvoke(DispatcherPriority.Normal,
                              new Action(() =>
                              {
                                LastError = x;
                                IsConnecting = false;
                              }));
    }

  }
}
