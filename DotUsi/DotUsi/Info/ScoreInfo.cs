using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DotUsi
{
  /// <summary>Keeps information about score engine sends</summary>
  public class ScoreInfo : INotifyPropertyChanged
  {
    private int _centiPawns;
    private int _mate;
    private bool _isLowerBound;
    private bool _isUpperBound;
    private readonly Dictionary<string, Action<string>> _parserTable;

    internal ScoreInfo()
    {
      _parserTable = InitInfoParserTable();
    }
    internal void ParseLine(string line)
    {
      InfoParserUtils.ParseLine(_parserTable, line);
    }

    /// <summary>The score from the engine's point of view, in centipawns.</summary>
    public int CentiPawns
    {
      get { return _centiPawns; }
      private set
      {
        if (_centiPawns == value) return;
        _centiPawns = value;
        RaisePropertyChanged("CentiPawns");
      }
    }
    /// <summary>Mate in y plies. If the engine is getting mated, use negative values for y.</summary>
    public int Mate
    {
      get { return _mate; }
      private set
      {
        if (_mate == value) return;
        _mate = value;
        RaisePropertyChanged("Mate");
      }
    }
    /// <summary>The score is just a lower bound.</summary>
    public bool IsLowerBound
    {
      get { return _isLowerBound; }
      private set
      {
        if (_isLowerBound == value) return;
        _isLowerBound = value;
        RaisePropertyChanged("IsLowerBound");
      }
    }
    /// <summary>The score is just an upper bound.</summary>
    public bool IsUpperBound
    {
      get { return _isUpperBound; }
      private set
      {
        if (_isUpperBound == value) return;
        _isUpperBound = value;
        RaisePropertyChanged("IsUpperBound");
      }
    }
    
    /// <summary>Occurs when a property value changes</summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private Dictionary<string, Action<string>> InitInfoParserTable()
    {
      return new Dictionary<string, Action<string>>
               {
                 {"cp", v => CentiPawns = int.Parse(v)},
                 {"mate", v => Mate = v == "-" ? 0 : int.Parse(v)},
                 {"lowerbound", v => IsLowerBound = true },
                 {"upperbound", v => IsUpperBound = true },
               };
    }
    private void RaisePropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}