using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore.Moves
{
  public class MovesHistory : ReadOnlyObservableCollection<MoveBase>
  {
    private int _currentMoveIndex = -1;

    public MovesHistory()
      : base(new ThreadSafeObservableCollection<MoveBase>())
    {
    }

    public void Do(MoveBase move)
    {
      if (move == null) throw new ArgumentNullException("move");

      while (!IsCurrentMoveLast)
        Items.RemoveAt(Count - 1);

      Items.Add(move);
      CurrentMoveIndex++;

      if (Items.Count == 1)
        OnPropertyChanged(new PropertyChangedEventArgs("IsEmpty"));
    }

    public MoveBase CurrentMove
    {
      get { return _currentMoveIndex < 0 ? null : Items[_currentMoveIndex]; }
      set
      {
        if (CurrentMove == value) return;

        if (value == null)
        {
          CurrentMoveIndex = -1;
          return;
        }

        int index = Items.IndexOf(value);
        if (index == -1) throw new ArgumentOutOfRangeException("value", 
          "You can only assign to MovesHistory.CurrentMove move from MovesHistory");

        CurrentMoveIndex = index;
      }
    }

    public int CurrentMoveIndex
    {
      get { return _currentMoveIndex; }
      set
      {
        if (_currentMoveIndex == value) return;
        if (value < -1 || value >= Count) 
          throw new ArgumentOutOfRangeException("value", 
            "Index must be grater or equal -1 and less than elements count");

        _currentMoveIndex = value;
        OnPropertyChanged(new PropertyChangedEventArgs("CurrentMoveIndex"));
        OnPropertyChanged(new PropertyChangedEventArgs("CurrentMove"));
        OnPropertyChanged(new PropertyChangedEventArgs("IsCurrentMoveLast"));
      }
    }

    public bool IsEmpty
    {
      get { return Items.Count == 0; }
    }

    public bool IsCurrentMoveLast
    {
      get { return _currentMoveIndex == Count - 1; }
    }

    public BoardSnapshot GetCurrentSnapshot()
    {
      if (CurrentMoveIndex == -1)
      {
        return Count > 0 ? this[0].BoardSnapshot : null;
      }
      if (CurrentMoveIndex < Count - 1)
      {
        return this[CurrentMoveIndex + 1].BoardSnapshot;
      }
      return new BoardSnapshot(
        CurrentMove.BoardSnapshot, CurrentMove.Snapshot());
    }

    public void GoToTheLast()
    {
      CurrentMoveIndex = Count - 1;
    }
  }
}