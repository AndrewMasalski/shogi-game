using System;
using Yasc.Networking.Interfaces;

namespace Yasc.Networking
{
  public partial class ShogiServer
  {
    private class ServerUser : MarshalByRefObject, IServerUser
    {
      private ServerGame _currentGame;

      public ServerUser(string name)
      {
        Name = name;
      }

      public string Name { get; private set; }

      IServerGame IServerUser.CurrentGame
      {
        get { return _currentGame; }
      }
    }
  }
}