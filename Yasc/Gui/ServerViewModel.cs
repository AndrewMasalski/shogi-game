using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.Networking;
using Yasc.Utils;

namespace Yasc.Gui
{
  public class ServerViewModel : ObservableObject
  {
    public IServerSession Session { get; internal set; }
    public Server Server { get; private set; }

    public IPlayerGameController GameTicket { get; set; }

    public ServerViewModel(IServerSession session)
    {
      Server = session.Server;
      Session = session;
      Init();
    }

    private void Init()
    {
      Users = new ObservableCollection<UserViewModel>(
        from u in Session.Users select new UserViewModel(Session, u));

      Users.CollectionChanged += UsersOnCollectionChanged;

      Games = new ObservableCollection<GameViewModel>(
        from g in Session.Games select new GameViewModel(g));
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
        _user.Invited += OnInvited;
        _user.InvitationAccepted += OnAccepted;
      }

      private void OnAccepted(IPlayerGameController obj)
      {
        _owner.OnInvitationAccepted(_user, obj);
      }

      private void OnInvited(object sender, EventArgs args)
      {
        _owner.OnUserInvited(_user);
      }

      public void Dispose()
      {
        _user.Invited -= OnInvited;
        _user.InvitationAccepted -= OnAccepted;
      }
    }

    private readonly List<IDisposable> _handlers = new List<IDisposable>();

    public void OnInvitationAccepted(UserViewModel user, IPlayerGameController controller)
    {
      InvokeGame(new InvitationAcceptedEventArgs(controller));
    }

    public void OnUserInvited(UserViewModel user)
    {

    }

    private void UsersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (UserViewModel user in args.NewItems)
            _handlers.Add(new UserHandler(this, user));
          break;

        case NotifyCollectionChangedAction.Reset:
          foreach (var handler in _handlers)
            handler.Dispose();
          _handlers.Clear();
          foreach (var user in Users)
            _handlers.Add(new UserHandler(this, user));
          break;
      }
    }

    protected ObservableCollection<GameViewModel> Games { get; private set; }

    public ServerViewModel(string userName)
    {
      Server = Server.Start();
      Session = Server.Login(userName);
      Init();
    }

    public ObservableCollection<UserViewModel> Users { get; private set; }

    public event EventHandler Disconnected;

    private void InvokeDisconnected(EventArgs e)
    {
      var handler = Disconnected;
      if (handler != null) handler(this, e);
    }

    public event EventHandler<InvitationAcceptedEventArgs> Game;

    private void InvokeGame(InvitationAcceptedEventArgs e)
    {
      var game = Game;
      if (game != null) game(this, e);
    }


    public event EventHandler GameNegotiation;

    public ICommand LogoutCommand
    {
      get
      {
        if (_logoutCommand == null)
        {
          _logoutCommand = new RelayCommand(Logout);
        }
        return _logoutCommand;
      }
    }

    private void Logout()
    {
      InvokeDisconnected(EventArgs.Empty);
    }

    private RelayCommand _logoutCommand;

    public ICommand RefreshGamesCommand
    {
      get
      {
        if (_refreshGamesCommand == null)
        {
          _refreshGamesCommand = new RelayCommand(RefreshGames);
        }
        return _refreshGamesCommand;
      }
    }

    private void RefreshGames()
    {
      Games.Update(Session.Games, null, g => new GameViewModel(g));
    }

    public ICommand UsersRefreshCommand
    {
      get
      {
        if (_usersRefreshCommand == null)
        {
          _usersRefreshCommand = new RelayCommand(UsersRefresh);
        }
        return _usersRefreshCommand;
      }
    }

    private void UsersRefresh()
    {
      Users.Update(Session.Users, null, g => new UserViewModel(Session, g));
    }

    private RelayCommand _usersRefreshCommand;


    private RelayCommand _refreshGamesCommand;

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
