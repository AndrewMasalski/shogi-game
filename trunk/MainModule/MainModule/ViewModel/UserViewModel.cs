using System;
using System.Windows.Input;
using Yasc.Networking.Interfaces;
using Yasc.Networking.Utils;
using Yasc.Utils.Mvvm;

namespace MainModule.ViewModel
{
  public class UserViewModel : ObservableObject
  {
    private readonly IServerSession _session;
    private readonly IServerUser _user;

    public UserViewModel(IServerSession session, IServerUser user)
    {
      _session = session;
      _user = user;
      Name = _user.Name;
    }

    public string Name { get; private set; }

    public ICommand InviteCommand
    {
      get { return _inviteCommand ?? (_inviteCommand = new RelayCommand(Invite)); }
    }

    private RelayCommand _inviteCommand;

    public void Invite()
    {
      _session.InvitePlay(_user,
        new ActionListener<IPlayerGameController>(OnInvitationAccepted));
    }

    public event Action<IPlayerGameController> InvitationAccepted;

    private void OnInvitationAccepted(IPlayerGameController obj)
    {
      var accepted = InvitationAccepted;
      if (accepted != null) accepted(obj);
    }
  }
}