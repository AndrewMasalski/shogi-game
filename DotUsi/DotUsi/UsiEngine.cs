using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Yasc.DotUsi.Drivers;
using Yasc.DotUsi.Exceptions;
using Yasc.DotUsi.Info;
using Yasc.DotUsi.Options;
using Yasc.DotUsi.Options.Base;
using Yasc.DotUsi.Process;
using Yasc.DotUsi.SearchModifiers;
using Yasc.DotUsi.SearchModifiers.Base;

namespace Yasc.DotUsi
{
  /// <summary>Represents USI compatible engine as described 
  ///   <a href="http://www.glaurungchess.com/shogi/usi.html">here</a></summary>
  public class UsiEngine : IDisposable
  {
    private bool _debugMode;
    private bool _isDisposing;
    private readonly IUsiProcess _process;
    private readonly List<UsiOptionBase> _options = new List<UsiOptionBase>();

    #region ' Public Properties '

    /// <summary>List of options the engine supports</summary>
    public ReadOnlyCollection<UsiOptionBase> Options
    {
      get { return new ReadOnlyCollection<UsiOptionBase>(_options); }
    }
    /// <summary>Identifies the engine, e.g. Shredder X.Y</summary>
    public string EngineName { get; private set; }
    /// <summary>Identifies the engine author, e.g. Stefan MK</summary>
    public string AuthorName { get; private set; }
    /// <summary>Current engine state</summary>
    public EngineMode Mode { get; private set; }
    /// <summary>Aggregates info engine sends to user</summary>
    public EngineInfo Info { get; private set; }
    /// <summary>Switch the debug mode of the engine on and off</summary>
    /// <remarks>
    /// <para>In debug mode the engine should send additional infos, 
    ///   e.g. with the info string command, to help debugging, 
    ///   e.g. the commands that the engine has received, etc. </para>
    /// <para>This mode is switched off by default and you can change it any time, 
    ///   also when the engine is thinking.</para>
    /// </remarks>
    public bool DebugMode
    {
      get { return _debugMode; }
      set
      {
        VerifyNotDisposed();
        if (_debugMode == value) return;
        _debugMode = value;
        var mode = _debugMode ? "on" : "off";
        _process.WriteLine("debug " + mode);
      }
    }

    #endregion

    /// <summary>ctor</summary>
    /// <param name="process">engine process</param>
    public UsiEngine(IUsiProcess process)
    {
      Info = new EngineInfo();
      _process = process;
      _process.OutputDataReceived += ReceiveOutputData;
      SetEngine();
    }

    #region ' Public Methods '

    /// <summary>
    /// Tell engine to use the USI (universal shogi interface). 
    /// This will be sent once as a first command after program boot to 
    /// tell the engine to switch to USI mode. After receiving the usi 
    /// command the engine must identify itself with the id command and 
    /// send the option commands to tell the GUI which engine settings the engine supports. 
    /// After that, the engine should send usiok to acknowledge the USI mode. 
    /// If no usiok is sent within a certain time period, the engine task will be killed by the GUI.
    /// </summary>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void Usi()
    {
      VerifyStarted();
      Mode = EngineMode.Usi;
      _process.WriteLine("usi");
    }
    /// <summary>This is used to synchronize the engine with the GUI.</summary>
    /// <remarks>
    /// <para>When the GUI has sent a command or multiple commands that 
    ///   can take some time to complete, this command can be used to 
    ///   wait for the engine to be ready again or to ping the engine 
    ///   to find out if it is still alive. </para>
    /// <para>This command is also required once before the engine is asked 
    ///   to do any search to wait for the engine to finish initializing.</para>
    /// <para>This command can be sent also when the engine is calculating 
    ///   in which case the engine should also immediately answer with 
    ///   readyok without stopping the search.</para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void IsReady()
    {
      VerifyCorrupted();
      Mode = EngineMode.Waiting;
      _process.WriteLine("isready");
    }

    /// <summary>Call this if the next <see cref="Position(string[])"/> 
    ///   is going to belong to a different game.</summary>
    /// <remarks>
    ///   <para>This can be a new game the engine should play or a new game it should analyse but 
    ///   also the next position from a testsuite with different positions.</para>
    ///   <para>As the engine's reaction to this command can take some time the GUI should always 
    ///   send use <see cref="IsReady"/> to wait for the engine to finish this operation.</para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void NewGame()
    {
      VerifyIsReady();
      Mode = EngineMode.Corrupted;
      _process.WriteLine("usinewgame");
    }
    /// <summary>Set up the default start position on
    ///   the internal board and play the <paramref name="moves"/></summary>
    /// <remarks>If this position is from a different game 
    ///   than the last position sent to the engine, 
    ///   the GUI should have sent a <see cref="NewGame"/> inbetween.</remarks>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void Position(params string[] moves)
    {
      VerifyIsReady();
      var command = new StringBuilder("position startpos");
      if (moves.Length > 0)
      {
        command.Append(" moves ");
        command.Append(string.Join(" ", moves));
      }
      _process.WriteLine(command.ToString());
    }
    /// <summary>Set up the position described in <paramref name="sfen"/> on 
    ///   the internal board and play the <paramref name="moves"/> 
    ///   on the internal board. </summary>
    /// <remarks>If this position is from a different game 
    ///   than the last position sent to the engine, 
    ///   the GUI should have sent a <see cref="NewGame"/> inbetween.</remarks>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void Position(SfenString sfen, params string[] moves)
    {
      if (sfen == null) throw new ArgumentNullException("sfen");
      VerifyIsReady();

      var command = new StringBuilder("position sfen ");
      command.Append(sfen);
      if (moves.Length > 0)
      {
        command.Append(" moves ");
        command.Append(string.Join(" ", moves));
      }
      _process.WriteLine(command.ToString());
    }
    /// <summary>Start calculating on the position set up with the position command.</summary>
    /// <param name="modifiers">List of search process modifiers</param>
    /// <exception cref="ArgumentNullException">One of arguments is null</exception>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void Go(params UsiSearchModifier[] modifiers)
    {
      if (modifiers.Where(m => m == null).Count() > 0)
        throw new ArgumentNullException("modifiers");

      VerifyIsReady();

      var command = new StringBuilder("go");
      foreach (var modifier in modifiers)
      {
        command.Append(" ");
        command.Append(modifier.Command);
      }
      _process.WriteLine(command.ToString());

      Mode = modifiers.OfType<PonderModifier>().Count() > 0 ?
        EngineMode.Pondering : EngineMode.Searching;
    }

    /// <summary>Stop calculating as soon as possible.</summary>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void Stop()
    {
      VerifySearchOrPondering();
      _process.WriteLine("stop");
      Mode = EngineMode.Ready;
    }
    /// <summary>The user has played the expected move. 
    ///   This will be sent if the engine was told to ponder on the same move the user has played.
    ///   The engine should continue searching but switch from pondering to normal search.
    /// </summary>
    /// <exception cref="InvalidOperationException">Method is called within wrong <see cref="Mode"/></exception>
    public void PonderHit()
    {
      VerifyPondering();
      _process.WriteLine("ponderhit");
      Mode = EngineMode.Searching;
    }

    /// <summary>Closes the engine process and releases all resources</summary>
    public void Dispose()
    {
      lock (this)
      {
        if (_isDisposing) return;
        _isDisposing = true;
        if (Mode == EngineMode.Disposed) return;
        // Don't want any asynch event to be fired after dispose is called
        BestMove = null;

        _process.WriteLine("quit");
        _process.Dispose();
        Mode = EngineMode.Disposed;
        _isDisposing = false;
      }
    }

    #endregion

    /// <summary>Raised when the best move is found by the engine</summary>
    public event EventHandler<BestMoveEventArgs> BestMove;
    /// <summary>Raised when engine said all it wanted to say after <see cref="Usi"/> command</summary>
    public event EventHandler UsiOK;
    /// <summary>Raised when engine confirms that it's ready after <see cref="IsReady"/> command</summary>
    public event EventHandler ReadyOK;

    #region ' VerifyMode methods '

    private void VerifyPondering()
    {
      VerifyNotDisposed();

      if (Mode != EngineMode.Pondering)
        throw new InvalidOperationException(
          "PonderHit opetarion can only be performed after GoPonder.");
    }
    private void VerifyStarted()
    {
      VerifyNotDisposed();

      if (Mode != EngineMode.Started)
        throw new InvalidOperationException(
          "You can't call Usi command twice");
    }
    private void VerifyIsReady()
    {
      VerifyNotDisposed();

      if (Mode != EngineMode.Ready)
        throw new InvalidOperationException(
          "You cannot call this command when engine is not ready.");
    }
    private void VerifyIsReadyOrCorrupted()
    {
      VerifyNotDisposed();

      if (Mode != EngineMode.Ready && Mode != EngineMode.Corrupted)
        throw new InvalidOperationException(
          "You cannot call this command when engine is not ready.");
    }
    private void VerifyCorrupted()
    {
      VerifyNotDisposed();

      if (Mode == EngineMode.Waiting)
        throw new InvalidOperationException("You must not call IsReady method twice");

      if (Mode != EngineMode.Corrupted)
        throw new InvalidOperationException(
          "You've no reason to call IsReady method: you didn't set options nor started new game");
    }
    private void VerifyNotDisposed()
    {
      if (Mode == EngineMode.Disposed)
        throw new InvalidOperationException(
          "You cannot call any command when engine is disposed.");
    }
    private void VerifySearchOrPondering()
    {
      VerifyNotDisposed();

      if (Mode != EngineMode.Searching && Mode != EngineMode.Pondering)
        throw new InvalidOperationException(
          "This operation cannot be performed when engine is not searching or pondering.");
    }

    #endregion

    private void ReceiveOutputData(object sender, LineReceivedEventArgs e)
    {
      if (e.Line == null)
      {
        // Process exited unexpectedly?
        Dispose();
        return;
      }

      switch (Mode)
      {
        case EngineMode.Usi:
          if (e.Line.StartsWith("id "))
          {
            ParseId(e.Line.Substring("id ".Length));
          }
          else if (e.Line.StartsWith("option "))
          {
            _options.Add(ParseOption(e.Line.Substring("option ".Length)));
          }
          else if (e.Line == "usiok")
          {
            LoadImplicitOptions();
            Mode = EngineMode.Ready;
            OnUsiOK(EventArgs.Empty);
          }
          break;
        case EngineMode.Waiting:
          if (e.Line == "readyok")
          {
            Mode = EngineMode.Ready;
            OnReadyOK(EventArgs.Empty);
          }
          break;
        case EngineMode.Searching:
          const string bestMove = "bestmove ";
          const string info = "info ";

          if (e.Line.StartsWith(bestMove))
            OnBestMove(ParseBestMove(e.Line.Substring(bestMove.Length)));
          if (e.Line.StartsWith(info))
            Info.ParseLine(e.Line.Substring(info.Length));
          break;
      }
    }

    #region ' Implementation '

    private void OnBestMove(BestMoveEventArgs e)
    {
      Mode = EngineMode.Ready;
      var handler = BestMove;
      if (handler != null) handler(this, e);
    }
    private void OnUsiOK(EventArgs e)
    {
      var handler = UsiOK;
      if (handler != null) handler(this, e);
    }
    private void OnReadyOK(EventArgs e)
    {
      var handler = ReadyOK;
      if (handler != null) handler(this, e);
    }
    
    private UsiOptionBase ParseOption(string line)
    {
      var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      string name = null;
      string optionType = null;
      string defaultValue = null;
      string min = null;
      string max = null;
      var possibleValues = new List<string>();
      for (int i = 0; i < split.Length; i++)
      {
        if (split[i] == "name")
        {
          name = split[++i];
        }
        else if (split[i] == "type")
        {
          optionType = split[++i];
        }
        else if (split[i] == "default")
        {
          defaultValue = split[++i];
        }
        else if (split[i] == "min")
        {
          min = split[++i];
        }
        else if (split[i] == "max")
        {
          max = split[++i];
        }
        else if (split[i] == "var")
        {
          possibleValues.Add(split[++i]);
        }
      }
      return CreateOptionObject(name, optionType, defaultValue, min, max, possibleValues);
    }
    private UsiOptionBase CreateOptionObject(string name, string optionType, string defaultValue, string min, string max, IList<string> possibleValues)
    {
      if (name == null) throw new UsiParserException("Option can't have no name");
      if (optionType == null) throw new UsiParserException("Option can't have no type");

      switch (optionType)
      {
        case "check":
          return new CheckOption(this, name, defaultValue);
        case "spin":
          return new SpinOption(this, name, defaultValue, min, max);
        case "combo":
          return new ComboOption(this, name, defaultValue, new ReadOnlyCollection<string>(possibleValues));
        case "button":
          return new ButtonOption(this, name);
        case "string":
          return new StringOption(this, name, defaultValue);
        case "filename":
          return new FileNameOption(this, name, defaultValue);
        default:
          throw new UsiParserException("Unrecognized option type: " + optionType);
      }
    }
 
    private void ParseId(string line)
    {
      if (line.StartsWith("name "))
        EngineName = line.Substring("name ".Length);
      else if (line.StartsWith("author "))
        AuthorName = line.Substring("author ".Length);
      // else throw new NotSupportedException("Engine output is not supported");
    }
    private static BestMoveEventArgs ParseBestMove(string line)
    {
      if (line == "resign") return new BestMoveEventArgs();
      var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      return new BestMoveEventArgs(split[0], (split.Length == 3 ? split[2] : null));
    }

    #endregion

    #region ' IEngineHook '

    private void SetEngine()
    {
      var hook = _process as IEngineHook;
      if (hook == null) return;
      hook.SetEngine(this);
    }
    private void LoadImplicitOptions()
    {
      var hook = _process as IEngineHook;
      if (hook == null) return;
      _options.AddRange(hook.GetImplicitOptions());
    }

    /// <summary>Sets mandatory implicit options if they are</summary>
    public void SetImplicitOptions()
    {
      var hook = _process as IEngineHook;
      if (hook == null) return;
      foreach (var option in Options)
        if (option.IsImplicit)
          option.SetImplicitValue();
    }

    #endregion


    /// <summary>This is sent to the engine to change its internal parameters.</summary>
    /// <param name="option">One from <see cref="Options"/> collection</param>
    /// <param name="needValue">true if command needs "value xxx" part</param>
    /// <remarks>
    /// <para>For the button type no value is needed (pass null). </para>
    /// <para>Can only set options when the engine is waiting. </para>
    /// <para>Can group setting options within on setoption command</para>
    /// </remarks>
    internal void SetOption(UsiOptionBase option, bool needValue)
    {
      VerifyIsReadyOrCorrupted();
      Mode = EngineMode.Corrupted;
      _process.WriteLine(option.CommitCommand);
    }
  }
}
/*
>1:usinewgame
>1:position startpos moves 7g7f
>1:go btime 0 wtime 60000 byoyomi 30000
<1:bestmove 4a3b ponder 6i7h
>1:position startpos moves 7g7f 4a3b 6i7h
>1:go ponder btime 0 wtime 59000 byoyomi 30000
>1:stop
<1:bestmove 1c1d ponder 5i6h
>1:position startpos moves 7g7f 4a3b 6g6f
>1:go btime 0 wtime 59000 byoyomi 30000
<1:info time 30 nodes 19098 score cp 1 pv 6a5b 6i7h 5a4b 5i6h
<1:bestmove 6a5b ponder 6i7h
>1:position startpos moves 7g7f 4a3b 6g6f 6a5b 6i7h
>1:go ponder btime 0 wtime 58000 byoyomi 30000
>1:ponderhit
<1:bestmove 1c1d ponder 5i6h
>1:position startpos moves 7g7f 4a3b 6g6f 6a5b 6i7h 1c1d 5i6h
>1:go ponder btime 0 wtime 57000 byoyomi 30000
<1:info currmove 5a4b
<1:info time 0 depth 1 nodes 3 score cp 2 pv 5a4b
<1:info currmove 5a4b
<1:info time 1 depth 2 nodes 127 score cp -1 pv 5a4b 4i5h
<1:info currmove 5a4b
<1:info currmove 5a4b
<1:info time 2 depth 3 nodes 1072 score cp 1 pv 5a4b 4i5h 1d1e
<1:info currmove 5a4b
<1:info currmove 5a4b
<1:info time 11 depth 4 nodes 5186 score cp -2 pv 5a4b 4i5h 1d1e 5h6g
<1:info currmove 5a4b
<1:info time 31 nodes 17660 score cp -2 pv 5a4b 4i5h 1d1e 5h6g 
 */