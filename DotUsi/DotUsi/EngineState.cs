using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DotUsi
{
  public class EngineState : INotifyPropertyChanged
  {
    private int _depth;
    private int _selectiveDepth;
    private TimeSpan _time;
    private int _nodes;
    private readonly ObservableCollection<string> _principalVariation;

    /// <summary>Search depth</summary>
    public int Depth
    {
      get { return _depth; }
      internal set
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
      internal set
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
      internal set
      {
        if (_time == value) return;
        _time = value;
        RaisePropertyChanged("Time");
      }
    }
    /// <summary>Nodes searched</summary>
    public int Nodes
    {
      internal get { return _nodes; }
      set
      {
        if (_nodes == value) return;
        _nodes = value;
        RaisePropertyChanged("Nodes");
      }
    }
    /// <summary>The best moves sequence found</summary>
    public ReadOnlyObservableCollection<string> PrincipalVariation { get; private set; }

    public EngineState()
    {
      _principalVariation = new ObservableCollection<string>();
      PrincipalVariation = new ReadOnlyObservableCollection<string>(_principalVariation);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal void UpdatePrincipalVariation(IEnumerable<string> newPrincVar)
    {
      _principalVariation.Clear();
      foreach (var pv in newPrincVar)
        _principalVariation.Add(pv);
    }
    private void RaisePropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}