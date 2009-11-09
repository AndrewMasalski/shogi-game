using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Windows;
using Yasc.ShogiCore.Utils;

namespace Yasc.Networking
{
  public class Server : MarshalByRefObject
  {
    private PlayerGameController _whitePlayer;
    private PlayerGameController _blackPlayer;
    private readonly List<SpectatorController>
      _spectators = new List<SpectatorController>();

    private readonly Random _rnd = new Random();

    public static bool ThisDomainIsServerHost { get; private set; }
    public static bool ServerIsStartedOnThisComputer
    { 
      get { return PortUtils.IsPortBusy(Port); }
    }
    public IPlayerGameController ParticipateGame()
    {
      if (_whitePlayer == null && _blackPlayer == null)
      {
        if (_rnd.Next(2) == 0)
        {
          return _whitePlayer = new PlayerGameController(this, PieceColor.White);
        }
        return _blackPlayer = new PlayerGameController(this, PieceColor.Black);
      }
      if (_whitePlayer == null)
      {
        return _whitePlayer = new PlayerGameController(this, PieceColor.White);
      }
      if (_blackPlayer == null)
      {
        return _blackPlayer = new PlayerGameController(this, PieceColor.Black);
      }
      return null;
    }
    public ISpectatorController WatchGame()
    {
      var spectator = new SpectatorController();
      _spectators.Add(spectator);
      return spectator;
    }
    public void Ping() { }
    private static int _port = 1937;
    public static int Port
    {
      get { return _port; }
      set
      {
        if (ThisDomainIsServerHost)
        {
          throw new InvalidOperationException(
            "Can't change server port when it is started");
        }
        if (_port == value) return;
        _port = value;
      }
    }

    private PlayerGameController Opponent(PlayerGameController controller)
    {
      var opponent = controller == _whitePlayer ? _blackPlayer : _whitePlayer;
      if (opponent == null) MessageBox.Show("Opponent is not found!");
      return opponent;
    }

    private void Move(PlayerGameController controller, string move)
    {
      Opponent(controller).InvokeOpponentMadeMove(move);

      foreach (var spectator in _spectators)
        spectator.InvokePlayerMadeMove(controller.MyColor, move);
    }

    public static void Start()
    {
      var provider = new BinaryServerFormatterSinkProvider
                       {
                         TypeFilterLevel = TypeFilterLevel.Full
                       };
      IDictionary properties = new Hashtable();
      properties["name"] = "some name";
      properties["port"] = Port;
      ChannelServices.RegisterChannel(new TcpChannel(properties, null, provider), false);

      RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
      RemotingServices.Marshal(new Server(), typeof(Server).FullName);

      ThisDomainIsServerHost = true;
    }

    #region ' SpectatorController '

    private class SpectatorController : MarshalByRefObject, ISpectatorController
    {
      public event Action<PieceColor, string> PlayerMadeMove;

      public void InvokePlayerMadeMove(PieceColor color, string move)
      {
        var handler = PlayerMadeMove;
        if (handler != null) handler(color, move);
      }

      public event Action<PieceColor, string> PlayerSomeoneSaidSomething;

      public void InvokePlayerSomeoneSaidSomething(PieceColor color, string move)
      {
        var handler = PlayerSomeoneSaidSomething;
        if (handler != null) handler(color, move);
      }
    }

    #endregion

    #region ' PlayerGameController '

    private class PlayerGameController : MarshalByRefObject, IPlayerGameController
    {
      private readonly Server _owner;
      private readonly PieceColor _color;

      public PlayerGameController(Server owner, PieceColor color)
      {
        _owner = owner;
        _color = color;
      }

      public PieceColor MyColor
      {
        get { return _color; }
      }

      public void Move(string move)
      {
        _owner.Move(this, move);
      }

      public void Say(string move)
      {
        throw new NotImplementedException();
      }

      public event Action<string> OpponentMadeMove;

      public void InvokeOpponentMadeMove(string move)
      {
        var handler = OpponentMadeMove;
        if (handler != null) handler(move);
      }

      public event Action<string> OpponentSaidSomething;

      public void InvokeOpponentSaidSomething(string obj)
      {
        Action<string> something = OpponentSaidSomething;
        if (something != null) something(obj);
      }
    }

    #endregion

    public static Server Connect(string url)
    {
      if (!ThisDomainIsServerHost)
      {
        var chnl = new TcpChannel(PortUtils.GetFreePortToListen());
        ChannelServices.RegisterChannel(chnl, false);
      }

      url = string.Format("tcp://{0}:{1}/{2}", url, Port, typeof(Server).FullName);
      return (Server)Activator.GetObject(typeof(Server), url);
    }
  }
}