using System;
using MvvmFoundation.Wpf;
using Yasc.Networking;

namespace Yasc.Gui
{
  public class UserViewModel : ObservableObject
  {
    private readonly IServerSession _session;
    private readonly IServerUser _user;
    private IInvitorTicket _ticket;

    public UserViewModel(IServerSession session, IServerUser user)
    {
      _session = session;
      _user = user;
    }

    public void Invite()
    {
      _ticket = _session.InvitePlay(_user);
      _ticket.InvitationAccepted += OnInvitationAccepted;

      InvokeInvited(EventArgs.Empty);
    }

    private void OnInvitationAccepted(IPlayerGameController obj)
    {
      _ticket.InvitationAccepted -= OnInvitationAccepted;
      _ticket = null;

      InvokeInvitationAccepted(obj);
    }

    public event EventHandler Invited;

    private void InvokeInvited(EventArgs e)
    {
      EventHandler invited = Invited;
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