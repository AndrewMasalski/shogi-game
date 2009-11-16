using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.Networking;
using Yasc.Utils;

namespace Yasc.Gui
{
  public class ServerViewModel : ObservableObject
  {
    private RelayCommand _logoutCommand;
    private RelayCommand _usersRefreshCommand;
    private RelayCommand _refreshGamesCommand;

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

    public bool IsServer { get; private set; }
    public string ServerAddress { get; private set; }
    public IServerSession Session { get; internal set; }
    public Server Server { get; private set; }
    public IPlayerGameController GameTicket { get; set; }
    public ObservableCollection<GameViewModel> Games { get; private set; }
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
      Server = Server.Start();
      Session = Server.Login(userName);
      Init();
    }

    public event EventHandler Disconnected;
    public event EventHandler<InvitationAcceptedEventArgs> Game;
    public event EventHandler GameNegotiation;

    private void Init()
    {
      Session.InvitationReceived += new ActionListener<IInviteeTicket>(OnInvitationReceived);

      Users = new ObservableCollection<UserViewModel>();
      Users.Update(Session.Users, null, u => new UserViewModel(Session, u));

      Games = new ObservableCollection<GameViewModel>(
        from g in Session.Games select new GameViewModel(g));
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

    private void Logout()
    {
      OnDisconnected(EventArgs.Empty);
    }
    private void RefreshGames()
    {
      Games.Update(Session.Games, null, g => new GameViewModel(g));
    }
    private void UsersRefresh()
    {
      Users.Update(Session.Users, null, g => new UserViewModel(Session, g));
    }

    private void OnInvitationReceived(IInviteeTicket ticket)
    {
      OnInvitationAccepted(ticket.Accept());
    }
    private void OnInvitationAccepted(IPlayerGameController controller)
    {
      OnGame(new InvitationAcceptedEventArgs(controller));
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


/*
 

 * * 
      Users.CollectionChanged += UsersOnCollectionChanged;
    private readonly List<IDisposable> _handlers = new List<IDisposable>();
    private void OnUserInvited(UserViewModel user)
    {

    }
 * 
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
* 
 */