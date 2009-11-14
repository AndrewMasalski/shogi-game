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
    private static int _port = 1937;
    private static readonly Random _rnd = new Random();
    private static bool _backwardChannelIsRegistred;

    private PlayerGameController _whitePlayer;
    private PlayerGameController _blackPlayer;
    private readonly List<SpectatorController>
      _spectators = new List<SpectatorController>();

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
    private void Move(PlayerGameController controller, MoveMsg move)
    {
      Opponent(controller).InvokeOpponentMadeMove(move);

      foreach (var spectator in _spectators)
        spectator.InvokePlayerMadeMove(controller.MyColor, move);
    }

    #region ' SpectatorController '

    private class SpectatorController : MarshalByRefObject, ISpectatorController
    {
      public event Action<PieceColor, MoveMsg> PlayerMadeMove;

      public void InvokePlayerMadeMove(PieceColor color, MoveMsg move)
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
      private readonly Server _server;
      private DateTime? _gotLastOpponentMoveAt;

      public TimeSpan TimeLeft { get; private set; }

      public PlayerGameController(Server owner, PieceColor color)
      {
        _server = owner;
        MyColor = color;
        TimeLeft = TimeSpan.FromMinutes(5);
      }

      public PieceColor MyColor { get; private set; }

      public void Move(MoveMsg move)
      {
        _server.Move(this, move);
        if (_gotLastOpponentMoveAt != null)
        {
          TimeLeft -= move.TimeStamp - (DateTime)_gotLastOpponentMoveAt;
        }
      }

      public void Say(string move)
      {
        throw new NotImplementedException();
      }

      public Func<MoveMsg, DateTime> OpponentMadeMove { private get; set; }

      public void InvokeOpponentMadeMove(MoveMsg move)
      {
        var handler = OpponentMadeMove;
        if (handler != null)
        {
          _gotLastOpponentMoveAt = handler(move);
        }
      }

      public event Action<string> OpponentSaidSomething;

      public void InvokeOpponentSaidSomething(string obj)
      {
        Action<string> something = OpponentSaidSomething;
        if (something != null) something(obj);
      }
    }

    #endregion

    public static Server Start()
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
      var server = new Server();
      RemotingServices.Marshal(server, typeof(Server).FullName);

      ThisDomainIsServerHost = true;
      return server;
    }
    public static Server Connect(string url)
    {
      if (!_backwardChannelIsRegistred && !ThisDomainIsServerHost)
      {
        // We need a channel to get messages back from the server
        // doesn't matter what port it will have
        var chnl = new TcpChannel(PortUtils.GetFreePortToListen());
        ChannelServices.RegisterChannel(chnl, false);
        _backwardChannelIsRegistred = true;
      }

      url = string.Format("tcp://{0}:{1}/{2}", url, Port, typeof(Server).FullName);
      return (Server)Activator.GetObject(typeof(Server), url);
    }
  }
}