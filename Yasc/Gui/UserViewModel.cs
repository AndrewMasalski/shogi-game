using System;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.Networking;

namespace Yasc.Gui
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
      get
      {
        if (_inviteCommand == null)
        {
          _inviteCommand = new RelayCommand(Invite);
        }
        return _inviteCommand;
      }
    }

    private RelayCommand _inviteCommand;

    public void Invite()
    {
      _session.InvitePlay(_user, 
        new ActionListener<IPlayerGameController>(OnInvitationAccepted));

      InvokeInvited(EventArgs.Empty);
    }

    private void OnInvitationAccepted(IPlayerGameController obj)
    {
      InvokeInvitationAccepted(obj);
    }

    public event EventHandler Invited;

    private void InvokeInvited(EventArgs e)
    {
      var invited = Invited;
      if (invited != null) invited(this, e);
    }

    public event Action<IPlayerGameController> InvitationAccepted;

    private void InvokeInvitationAccepted(IPlayerGameController obj)
    {
      var accepted = InvitationAccepted;
      if (accepted != null) accepted(obj);
    }
  }
}