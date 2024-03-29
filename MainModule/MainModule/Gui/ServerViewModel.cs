﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using MainModule.Gui.Game;
using MainModule.ViewModel;
using Yasc.Networking;
using Yasc.Networking.Interfaces;
using Yasc.Networking.Utils;
using Yasc.Utils;
using Yasc.Utils.Mvvm;

namespace MainModule.Gui
{
  public class ServerViewModel : ObservableObject
  {
    private RelayCommand _logOffCommand;
    private RelayCommand _usersRefreshCommand;
    private RelayCommand _refreshGamesCommand;

    private readonly List<IDisposable> _handlers = new List<IDisposable>();

    public ICommand LogOffCommand
    {
      get { return _logOffCommand ?? (_logOffCommand = new RelayCommand(LogOff)); }
    }
    public ICommand RefreshGamesCommand
    {
      get { return _refreshGamesCommand ?? (_refreshGamesCommand = new RelayCommand(RefreshGames)); }
    }
    public ICommand UsersRefreshCommand
    {
      get { return _usersRefreshCommand ?? (_usersRefreshCommand = new RelayCommand(UsersRefresh)); }
    }

    public bool IsServer { get; private set; }
    public string ServerAddress { get; private set; }
    public IServerSession Session { get; internal set; }
    public ShogiServer Server { get; private set; }
    public IPlayerGameController GameTicket { get; set; }
    public ObservableCollection<GameItemViewModel> Games { get; private set; }
    public ObservableCollection<UserViewModel> Users { get; private set; }

    public ServerViewModel(string serverAddress, IServerSession session)
    {
      Server = session.Server;
      ServerAddress = serverAddress;
      Session = session;
      Init();
    }
    public ServerViewModel(string userName)
    {
      IsServer = true;
      Server = ShogiServer.Start();
      Session = Server.LogOn(userName);
      Init();
    }

    public event EventHandler Disconnected;
    public event EventHandler<InvitationAcceptedEventArgs> Game;
    public event EventHandler GameNegotiation;

    private void Init()
    {
      Session.InvitationReceived += new ActionListener<IInviteeTicket>(OnInvitationReceived);

      InitUsersCollection();

      Games = new ObservableCollection<GameItemViewModel>(
        from g in Session.Games select new GameItemViewModel(g));
    }

    private void InitUsersCollection()
    {
      Users = new ObservableCollection<UserViewModel>();
      Users.CollectionChanged += OnUsersCollectionChanged;
      Users.Update(Session.Users, iu => iu.Name, uvm => uvm.Name, 
        u => new UserViewModel(Session, u));
    }

    private void OnDisconnected(EventArgs e)
    {
      var handler = Disconnected;
      if (handler != null) handler(this, e);
    }
    private void OnGame(InvitationAcceptedEventArgs e)
    {
      var game = Game;
      if (game != null) game(this, e);
    }

    private void LogOff()
    {
      OnDisconnected(EventArgs.Empty);
    }
    private void RefreshGames()
    {
      Games.Update(Session.Games, 
        givm => givm.ServerGame, 
        ig => ig,
        ig => new GameItemViewModel(ig));
    }
    private void UsersRefresh()
    {
      Users.Update(Session.Users, iu => iu.Name, uvm => uvm.Name,
        u => new UserViewModel(Session, u));
    }

    private void OnInvitationReceived(IInviteeTicket ticket)
    {
      OnInvitationAccepted(ticket.Accept());
    }
    private void OnInvitationAccepted(IPlayerGameController controller)
    {
      OnGame(new InvitationAcceptedEventArgs(controller));
    }
    private void OnUsersCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          OnUserAdded(args);
          break;
        case NotifyCollectionChangedAction.Reset:
          OnUsersCollectionReset();
          break;
      }
    }

    private void OnUsersCollectionReset()
    {
      foreach (var handler in _handlers)
        handler.Dispose();

      _handlers.Clear();
      
      foreach (var user in Users)
        _handlers.Add(new UserHandler(this, user));
    }

    private void OnUserAdded(NotifyCollectionChangedEventArgs args)
    {
      foreach (UserViewModel user in args.NewItems)
        _handlers.Add(new UserHandler(this, user));
    }

    private struct UserHandler : IDisposable
    {
      private readonly ServerViewModel _owner;
      private readonly UserViewModel _user;

      public UserHandler(ServerViewModel owner, UserViewModel user)
        : this()
      {
        _owner = owner;
        _user = user;
        _user.InvitationAccepted += OnAccepted;
      }

      private void OnAccepted(IPlayerGameController controller)
      {
        _owner.OnInvitationAccepted(controller);
      }

      public void Dispose()
      {
        _user.InvitationAccepted -= OnAccepted;
      }
    }
  }

  public class InvitationAcceptedEventArgs : EventArgs
  {
    public IPlayerGameController Ticket { get; private set; }

    public InvitationAcceptedEventArgs(IPlayerGameController ticket)
    {
      Ticket = ticket;
    }
  }
}