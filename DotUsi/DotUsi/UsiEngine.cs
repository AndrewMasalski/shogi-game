using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DotUsi
{
  public class UsiEngine : IDisposable
  {
    private bool _debugMode;
    private readonly IUsiProcess _process;
    private readonly List<UsiOption> _options = new List<UsiOption>();
    private readonly EngineState _state = new EngineState();

    public ReadOnlyCollection<UsiOption> Options
    {
      get { return new ReadOnlyCollection<UsiOption>(_options); }
    }
    public string EngineName { get; private set; }
    public string AuthorName { get; private set; }
    public bool IsDisposed { get; private set; }
    public EngineMode Mode { get; private set; }
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
        if (_debugMode == value) return;
        _debugMode = value;
        var mode = _debugMode ? "on" : "off";
        _process.WriteLine("debug " + mode);
      }
    }

    public UsiEngine(IUsiProcess process)
    {
      _process = process;
      process.OutputDataReceived += OnOutputDataReceived;
    }

    public void Usi()
    {
      if (Mode == EngineMode.Ready)
        throw new InvalidOperationException("You can't call Usi command twice");
      _process.WriteLine("usi");
      IsReady();
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
    private void IsReady()
    {
      VerifyNotPing();
      Mode = EngineMode.Processing;
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
    public void NewGame()
    {
      VerifyIsReady();
      _process.WriteLine("usinewgame");
      IsReady();
    }
    /// <summary>Set up the default start position on
    ///   the internal board and play the <paramref name="moves"/></summary>
    /// <remarks>If this position is from a different game 
    ///   than the last position sent to the engine, 
    ///   the GUI should have sent a <see cref="NewGame"/> inbetween.</remarks>
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
    public void Go()
    {
      Go(null, null, null);
    }
    public void Go(TimeConstraint timeConstraint)
    {
      Go(null, timeConstraint, null);
    }
    public void Go(DepthConstraint depthConstraint)
    {
      Go(null, null, depthConstraint);
    }
    public void Go(UsiSearchModifier searchModifier)
    {
      Go(searchModifier, null, null);
    }
    public void Go(UsiSearchModifier searchModifier, TimeConstraint timeConstraint)
    {
      Go(searchModifier, timeConstraint, null);
    }
    public void Go(UsiSearchModifier searchModifier, DepthConstraint depthConstraint)
    {
      Go(searchModifier, null, depthConstraint);
    }
    public void Go(TimeConstraint timeConstraint, DepthConstraint depthConstraint)
    {
      Go(null, timeConstraint, depthConstraint);
    }
    public void Go(UsiSearchModifier searchModifier, TimeConstraint timeConstraint, DepthConstraint depthConstraint)
    {
      VerifyIsReady();

      if (timeConstraint == null && depthConstraint == null)
      {
        timeConstraint = TimeConstraint.InfiniteConstraint;
      }
      var command = new StringBuilder("go");
      if (searchModifier != null)
      {
        command.Append(" ");
        command.Append(searchModifier.ToString());
      }
      if (timeConstraint != null)
      {
        command.Append(" ");
        command.Append(timeConstraint.ToString());
      }
      if (depthConstraint != null)
      {
        command.Append(" ");
        command.Append(depthConstraint.ToString());
      }
      _process.WriteLine(command.ToString());
      Mode = EngineMode.Searching;
    }
  
    /// <summary><para>Start searching in pondering mode.</para> 
    ///   <para>This means that the last move X sent in the current position is the move to ponder on. 
    ///   The engine can do what it wants to do, but after a <see cref="PonderHit"/> command 
    ///   it should continue with move X. </para>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>This means that the ponder move sent by the GUI can be interpreted as a recommendation about which move to ponder on.
    /// However, if the engine decides to ponder on a different move, it should not display any mainlines as they are likely 
    /// to be misinterpreted by the GUI because the GUI expects the engine to ponder on the suggested move.</para>
    /// <para>Engine won't exit the search in ponder mode, even if it's mate!</para></remarks>
    public void GoPonder()
    {
      VerifyIsReady();
      _process.WriteLine("go ponder");
      Mode = EngineMode.Pondering;
    }

    /// <summary>Stop calculating as soon as possible.</summary>
    public void Stop()
    {
      VerifySearchState();
      _process.WriteLine("stop");
    }
    /// <summary>The user has played the expected move. 
    ///   This will be sent if the engine was told to ponder on the same move the user has played.
    ///   The engine should continue searching but switch from pondering to normal search.
    /// </summary>
    public void PonderHit()
    {
      if (Mode != EngineMode.Pondering)
        throw new InvalidOperationException(
          "PonderHit opetarion can only be performed after GoPonder.");
      _process.WriteLine("ponderhit");
      Mode = EngineMode.Ready;
    }
    /// <summary>This is sent to the engine to change its internal parameters.</summary>
    /// <param name="option">One from <see cref="Options"/> collection</param>
    /// <param name="value">Not case sensitive, cannot contain spaces</param>
    /// <remarks>
    /// <para>For the button type no value is needed (pass null). </para>
    /// <para>Can only set options when the engine is waiting. </para>
    /// <para>Can group setting options within on setoption command</para>
    /// </remarks>
    public void SetOption(UsiOption option, string value)
    {
      VerifyIsReady();
      _process.WriteLine("setoption " + option.Name + " " + value);
    }
    public void Dispose()
    {
      lock (this)
      {
        if (IsDisposed) return;
        // Don't want any asynch event to be fired after dispose is called
        BestMove = null;
        
        _process.WriteLine("quit");
        _process.Dispose();
        IsDisposed = true;
      }
    }
    
    public event EventHandler<BestMoveEventArgs> BestMove;

    private void OnOutputDataReceived(object sender, LineReceivedEventArgs e)
    {
      if (e.Line == null)
      {
        // Process exited unexpectedly?
        Dispose();
        return;
      }

      switch (Mode)
      {
        case EngineMode.Processing:
          if (e.Line.StartsWith("id "))
          {
            ParseId(e.Line.Substring("id ".Length));
          }
          else if (e.Line.StartsWith("option "))
          {
            _options.Add(ParseOption(e.Line.Substring("option ".Length)));
          }
          else if (e.Line == "readyok")
          {
            Mode = EngineMode.Ready;
            //            OnReady(EventArgs.Empty);
          }
          break;
        case EngineMode.Searching:
          {
            const string bestMove = "bestmove ";
            const string info = "info ";

            if (e.Line.StartsWith(bestMove))
              OnBestMove(ParseBestMove(e.Line.Substring(info.Length)));
            if (e.Line.StartsWith(info))
              ParseInfo(e.Line.Substring(info.Length));
          }
          break;
      }
    }
    private void VerifyIsReady()
    {
      if (Mode != EngineMode.Ready)
        throw new InvalidOperationException(
          "You cannot call this command when engine is not ready.");
    }
    private void VerifyNotPing()
    {
      if (Mode == EngineMode.Processing)
        throw new InvalidOperationException(
          "You cannot call this command when waiting for IsReady to respond. " +
          "If engine doesn't respond you can terminate it calling Dispose.");
    }
    private void VerifySearchState()
    {
      if (Mode != EngineMode.Searching)
        throw new InvalidOperationException(
          "This operation cannot be performed when engine is idle.");
    }

    private void OnBestMove(BestMoveEventArgs e)
    {
      var handler = BestMove;
      if (handler != null) handler(this, e);
    }
    private static UsiOption ParseOption(string line)
    {
      var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      string name = null;
      UsiOptionType? optionType = null;
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
          switch (split[++i])
          {
            case "check":
              optionType = UsiOptionType.Check;
              break;
            case "spin":
              optionType = UsiOptionType.Spin;
              break;
            case "combo":
              optionType = UsiOptionType.Combo;
              break;
            case "button":
              optionType = UsiOptionType.Button;
              break;
            case "string":
              optionType = UsiOptionType.String;
              break;
            case "filename":
              optionType = UsiOptionType.FileName;
              break;
          }
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
      if (name == null) throw new UsiParserError("Option can't have no name");
      if (optionType == null) throw new UsiParserError("Option can't have no type");
      return new UsiOption(name, (UsiOptionType)optionType, defaultValue, min, max, new ReadOnlyCollection<string>(possibleValues));
    }
    private void ParseInfo(string line)
    {
      var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < split.Length; i++)
      {
        if (split[i] == "depth")
        {
          _state.Depth = int.Parse(split[++i]);
        }
        else if (split[i] == "seldepth")
        {
          _state.SelectiveDepth = int.Parse(split[++i]);
        }
        else if (split[i] == "nodes")
        {
          _state.Nodes = int.Parse(split[++i]);
        }
        else if (split[i] == "time")
        {
          _state.Time = TimeSpan.FromMilliseconds(int.Parse(split[++i]));
        }
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
      var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      return new BestMoveEventArgs(split[0], (split.Length == 3 ? split[2] : null));
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