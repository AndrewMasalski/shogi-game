using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Yasc.ShogiCore.Moves;
using Yasc.Utils;

namespace Yasc.ShogiCore.Utils
{
  public class MovesHistory : ViewModelBase
  {
    private readonly ObservableCollection<MoveBase> _movesDone;

    public ReadOnlyObservableCollection<MoveBase> MovesDone
    {
      get { return new ReadOnlyObservableCollection<MoveBase>(_movesDone); }
    }

    public ReadOnlyObservableCollection<MoveBase> MovesUndone
    {
      get { return new ReadOnlyObservableCollection<MoveBase>(_movesUndone); }
    }

    public CompositeCollection AllMoves
    {
      get { return _allMoves; }
    }

    private readonly ObservableCollection<MoveBase> _movesUndone;
    private readonly CompositeCollection _allMoves;

    public MovesHistory()
    {
      _movesDone = new ThreadSafeObservableCollection<MoveBase>();
      _movesUndone = new ThreadSafeObservableCollection<MoveBase>();
      _allMoves = new CompositeCollection { MovesDone, MovesUndone };
    }

    internal void Do(MoveBase move)
    {
      _movesDone.Add(move);
      _movesUndone.Clear();
    }
    internal void Undo()
    {
      if (_movesDone.Count == 0) throw new InvalidOperationException("Cannot undo: history is empty");

      var move = _movesDone[_movesDone.Count - 1];
      _movesDone.RemoveAt(_movesDone.Count - 1);
      _movesUndone.Insert(0, move);
    }
    internal void Redo()
    {
      if (_movesUndone.Count == 0) throw new InvalidOperationException("Cannot redo: there are no further moves");

      var move = _movesUndone[0];
      _movesUndone.RemoveAt(0);
      _movesDone.Add(move);
    }
  }
}