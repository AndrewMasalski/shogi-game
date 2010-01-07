using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DotUsi
{
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
    private void RaisePropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private Dictionary<string, Action<string>> InitInfoParserTable()
    {
      return new Dictionary<string, Action<string>>
               {
                 {"cp", v => CentiPawns = int.Parse(v)},
                 {"mate", v => Mate = int.Parse(v)},
                 {"lowerbound", v => IsLowerBound = true },
                 {"upperbound", v => IsUpperBound = true },
               };
    }
  }
}