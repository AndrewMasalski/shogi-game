using System;
using MainModule.Gui;
using MainModule.Gui.Game;
using MainModule.Properties;
using MainModule.Utils;
using Yasc.Networking;
using Yasc.Networking.Interfaces;
using Yasc.Utils.Mvvm;

namespace MainModule
{
  public sealed class MainWindowViewModel : ObservableObject, IDisposable
  {
    private ObservableObject _currentView;

    public ObservableObject CurrentView
    {
      get { return _currentView; }
      private set
      {
        if (_currentView == value) return;
        _currentView = value;
        RaisePropertyChanged("CurrentView");
      }
    }

    public MainWindowViewModel()
    {
      switch (Settings.Default.DefaultStartMode)
      {
        case WelcomeChoice.None:
          GoWelcome();
          break;
        case WelcomeChoice.ArtificialIntelligence:
          GoPlayWithComp();
          break;
        case WelcomeChoice.Autoplay:
          GoAutoplay();
          break;
        case WelcomeChoice.BecomeServer:
          AutoBecomeServer();
          break;
        case WelcomeChoice.ConnectToServer:
          AutoConnectToServer();
          break;
      }
    }

    private void AutoConnectToServer()
    {
      string userName = Settings.Default.UserName;
      string address = Settings.Default.Address;
      if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(userName))
      {
        GoWelcome();
      }
      else
      {
        GoConnecting(new ConnectingViewModel(address, userName));
      }
    }

    private void AutoBecomeServer()
    {
      string userName = Settings.Default.UserName;
      if (string.IsNullOrEmpty(userName) || ShogiServer.IsServerStartedOnThisComputer)
      {
        GoWelcome();
      }
      else
      {
        GoServer(userName);
      }
    }

    public void Dispose()
    {
      var currentView = CurrentView as IDisposable;
      if (currentView != null)
        currentView.Dispose();
    }

    private void WelcomeOnChoiceDone(object sender, EventArgs args)
    {
      var welcomeViewModel = LeaveWelcome(sender);

      switch (welcomeViewModel.Mode)
      {
        case WelcomeChoice.ArtificialIntelligence:
          GoPlayWithComp();
          break;
        case WelcomeChoice.Autoplay:
          GoAutoplay();
          break;
        case WelcomeChoice.ConnectToServer:
          GoConnecting(new ConnectingViewModel(
            welcomeViewModel.Address, welcomeViewModel.UserName));
          break;
        case WelcomeChoice.BecomeServer:
          GoServer(welcomeViewModel.UserName);
          break;
      }
    }
    private void ConnectingOnFail(object sender, EventArgs e)
    {
      LeaveConnecting(sender);
      GoWelcome();
    }
    private void ConnectingOnSucceed(object sender, EventArgs e)
    {
      var connectingViewModel = LeaveConnecting(sender);
      GoServer(connectingViewModel.Address, connectingViewModel.Session);
    }

    private void OnDisconnected(object sender, EventArgs args)
    {
      LeaveServer(sender);
      GoWelcome();
    }
    private void OnGameOver(object sender, EventArgs args)
    {
      LeaveGame(sender);
      GoWelcome();
    }
    private void OnGameNegotiation(object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }
    private void OnGame(object sender, InvitationAcceptedEventArgs e)
    {
      LeaveServer(sender);
      GoGame(e.Ticket);
    }

    private void GoWelcome()
    {
      var welcomeViewModel = new WelcomeViewModel();
      welcomeViewModel.ChoiceDone += WelcomeOnChoiceDone;
      CurrentView = welcomeViewModel;
    }
    private void GoConnecting(ConnectingViewModel connectingViewModel)
    {
      connectingViewModel.Fail += ConnectingOnFail;
      connectingViewModel.Succeed += ConnectingOnSucceed;
      CurrentView = connectingViewModel;
    }
    private void GoServer(string userName)
    {
      var serverViewModel = new ServerViewModel(userName);
      serverViewModel.Disconnected += OnDisconnected;
      serverViewModel.Game += OnGame;
      serverViewModel.GameNegotiation += OnGameNegotiation;
      CurrentView = serverViewModel;
    }
    private void GoServer(string serverAddress, IServerSession session)
    {
      var serverViewModel = new ServerViewModel(serverAddress, session);
      serverViewModel.Disconnected += OnDisconnected;
      serverViewModel.Game += OnGame;
      serverViewModel.GameNegotiation += OnGameNegotiation;
      CurrentView = serverViewModel;
    }
    private void GoPlayWithComp()
    {
      var gameViewModel = GameWithEngineViewModel.Create();
      if (gameViewModel != null)
      {
        gameViewModel.GameOver += OnGameOver;
        CurrentView = gameViewModel;
      }
      else
      {
        GoWelcome();
      }
    }

    private void GoAutoplay()
    {
      var gameViewModel = new AutoplayViewModel();
      gameViewModel.GameOver += OnGameOver;
      CurrentView = gameViewModel;
    }
    private void GoGame(IPlayerGameController ticket)
    {
      var gameViewModel = new GameWithHumanViewModel(ticket);
      gameViewModel.GameOver += OnGameOver;
      CurrentView = gameViewModel;
    }

    private WelcomeViewModel LeaveWelcome(object sender)
    {
      var welcomeViewModel = (WelcomeViewModel)sender;
      welcomeViewModel.ChoiceDone -= WelcomeOnChoiceDone;
      return welcomeViewModel;
    }
    private ConnectingViewModel LeaveConnecting(object sender)
    {
      var connectingViewModel = (ConnectingViewModel)sender;
      connectingViewModel.Fail -= ConnectingOnFail;
      connectingViewModel.Succeed -= ConnectingOnSucceed;
      return connectingViewModel;
    }
    private void LeaveServer(object sender)
    {
      var serverViewModel = (ServerViewModel)sender;
      serverViewModel.Disconnected -= OnDisconnected;
      serverViewModel.Game -= OnGame;
      serverViewModel.GameNegotiation -= OnGameNegotiation;
    }
    private void LeaveGame(object sender)
    {
      var gameViewModel = (GameViewModel)sender;
      gameViewModel.GameOver -= OnGameOver;
      
      var disp = gameViewModel as IDisposable;
      if (disp != null) disp.Dispose();
    }
  }
}
