using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Yasc.DotUsi.Info
{
  /// <summary>Holds all information fields engine can notify about</summary>
  public class EngineInfo : INotifyPropertyChanged
  {
    private string _currentMove;
    private int _depth;
    private int _selectiveDepth;
    private TimeSpan _time;
    private int _nodes;
    private int _currentMoveNumber;
    private int _hashFull;
    private int _nodesPerSecond;
    private int _cpuLoad;
    private string _stringInfo;
    private readonly ObservableCollection<string> _refutation;
    private readonly ObservableCollection<string> _principalVariation;

    private readonly Dictionary<string, Action<string>> _parserTable;

    /// <summary>Search depth</summary>
    public int Depth
    {
      get { return _depth; }
      private set
      {
        if (_depth == value) return;
        _depth = value;
        RaisePropertyChanged("Depth");
      }
    }
    /// <summary>Selective search depth</summary>
    public int SelectiveDepth
    {
      get { return _selectiveDepth; }
      private set
      {
        if (_selectiveDepth == value) return;
        _selectiveDepth = value;
        RaisePropertyChanged("SelectiveDepth");
      }
    }
    /// <summary>The search time</summary>
    public TimeSpan Time
    {
      get { return _time; }
      private set
      {
        if (_time == value) return;
        _time = value;
        RaisePropertyChanged("Time");
      }
    }
    /// <summary>Nodes searched</summary>
    public int Nodes
    {
      get { return _nodes; }
      private set
      {
        if (_nodes == value) return;
        _nodes = value;
        RaisePropertyChanged("Nodes");
      }
    }
    /// <summary>Currently searching this move.</summary>
    public string CurrentMove
    {
      get { return _currentMove; }
      set
      {
        if (_currentMove == value) return;
        _currentMove = value;
        RaisePropertyChanged("CurrentMove");
      }
    }
    /// <summary>Currently searching move number x, for the first move x should be 1, not 0</summary>
    public int CurrentMoveNumber
    {
      get { return _currentMoveNumber; }
      set
      {
        if (_currentMoveNumber == value) return;
        _currentMoveNumber = value;
        RaisePropertyChanged("CurrentMoveNumber");
      }
    }
    /// <summary>The hash is x permill full. The engine should send this info regularly.</summary>
    public int HashFull
    {
      get { return _hashFull; }
      set
      {
        if (_hashFull == value) return;
        _hashFull = value;
        RaisePropertyChanged("HashFull");
      }
    }
    /// <summary>x nodes per second searched. the engine should send this info regularly</summary>
    public int NodesPerSecond
    {
      get { return _nodesPerSecond; }
      set
      {
        if (_nodesPerSecond == value) return;
        _nodesPerSecond = value;
        RaisePropertyChanged("NodesPerSecond");
      }
    }
    /// <summary>The cpu usage of the engine is x permill.</summary>
    public int CpuLoad
    {
      get { return _cpuLoad; }
      set
      {
        if (_cpuLoad == value) return;
        _cpuLoad = value;
        RaisePropertyChanged("CpuLoad");
      }
    }
    /// <summary>
    /// Any string which will be displayed be the engine. 
    /// TODO: If there is a string command the rest of the line will be interpreted as value
    /// </summary>
    public string StringInfo
    {
      get { return _stringInfo; }
      set
      {
        if (_stringInfo == value) return;
        _stringInfo = value;
        RaisePropertyChanged("StringInfo");
      }
    }
    /// <summary>The best moves sequence found</summary>
    public ReadOnlyObservableCollection<string> PrincipalVariation { get; private set; }
    /// <summary>Move 'move_1' is refuted by the line 'move_2' ... 'move_i', 
    /// where i can be any number> = 1. 
    /// Example: after move 8h2b+ is searched, the engine can send info refutation 8h2b+ 1c2b 
    /// if 1c2b is the best answer after 8h2b+ 
    /// or if 1c2b refutes the move 8h2b+.
    /// If there is no refutation for 8h2b+ found, the engine should just send info refutation 8h2b+. 
    /// The engine should only send this if the option USI_ShowRefutations is set to true.
    /// </summary>
    public ReadOnlyObservableCollection<string> Refutation { get; private set; }
    /// <summary>Score related info</summary>
    public ScoreInfo Score { get; private set; }

    internal EngineInfo()
    {
      _principalVariation = new ObservableCollection<string>();
      PrincipalVariation = new ReadOnlyObservableCollection<string>(_principalVariation);

      _refutation = new ObservableCollection<string>();
      Refutation = new ReadOnlyObservableCollection<string>(_refutation);

      Score = new ScoreInfo();

      _parserTable = InitInfoParserTable();
    }

    internal void ParseLine(string line)
    {
      InfoParserUtils.ParseLine(_parserTable, line);
    }

    /// <summary>Occurs when a property value changes.</summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private Dictionary<string, Action<string>> InitInfoParserTable()
    {
      return new Dictionary<string, Action<string>>
                  {
                    {"depth", v => Depth = int.Parse(v)},
                    {"seldepth", v => SelectiveDepth = int.Parse(v)},
                    {"nodes", v => Nodes = int.Parse(v)},
                    {"time", v => Time = TimeSpan.FromMilliseconds(int.Parse(v))},
                    {"pv", v => UpdatePrincipalVariation(v.Split(' '))},
                    {"currmove", v => CurrentMove = v},
                    {"currmovenumber", v => CurrentMoveNumber = int.Parse(v)},
                    {"hashfull", v => HashFull = int.Parse(v)},
                    {"nps", v => NodesPerSecond = int.Parse(v)},
                    {"cpuload", v => CpuLoad = int.Parse(v)},
                    {"refutation", v => UpdateRefutation(v.Split(' '))},
                    {"currline", v => { throw new NotImplementedException(); }},
                    {"score", v => Score.ParseLine(v) },
                  };
    }
    private void UpdatePrincipalVariation(IEnumerable<string> newPrincVar)
    {
      _principalVariation.Clear();
      foreach (var pv in newPrincVar)
        _principalVariation.Add(pv);
    }
    private void UpdateRefutation(IEnumerable<string> newRefutation)
    {
      _refutation.Clear();
      foreach (var m in newRefutation)
        _refutation.Add(m);
    }
    private void RaisePropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}