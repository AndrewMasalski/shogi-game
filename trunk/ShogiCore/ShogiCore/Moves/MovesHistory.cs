using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore.Moves
{
  /// <summary>Represents history of moves of the game</summary>
  public class MovesHistory : ReadOnlyObservableCollection<DecoratedMove>
  {
    private int _currentMoveIndex = -1;

    /// <summary>ctor</summary>
    public MovesHistory()
      : base(new ThreadSafeObservableCollection<DecoratedMove>())
    {
    }

    /// <summary>Adds the move to the history</summary>
    public void Add(DecoratedMove move)
    {
      if (move == null) throw new ArgumentNullException("move");

      while (!IsCurrentMoveLast)
        Items.RemoveAt(Count - 1);

      Items.Add(move);
      CurrentMoveIndex++;

      if (Items.Count == 1)
        OnPropertyChanged(new PropertyChangedEventArgs("IsEmpty"));
    }

    /// <summary>Adds the move to the history</summary>
    public void Add(Move snapshot)
    {
      if (snapshot == null) throw new ArgumentNullException("snapshot");

      Add(Decorate(snapshot));
    }

    /// <summary>Gets or sets currebt move reference</summary>
    public DecoratedMove CurrentMove
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

        var index = Items.IndexOf(value);
        if (index == -1) throw new ArgumentOutOfRangeException("value",
          "You can only assign to MovesHistory.CurrentMove move from MovesHistory");

        CurrentMoveIndex = index;
      }
    }

    /// <summary>Gets or sets currebt move index</summary>
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
        OnPropertyChanged(new PropertyChangedEventArgs("CurrentSnapshot"));
        OnPropertyChanged(new PropertyChangedEventArgs("IsCurrentMoveLast"));
      }
    }

    /// <summary>Indicates whether the history is empty</summary>
    public bool IsEmpty
    {
      get { return Items.Count == 0; }
    }
    
    /// <summary>Indicates whether the current move is last</summary>
    public bool IsCurrentMoveLast
    {
      get { return _currentMoveIndex == Count - 1; }
    }

    /// <summary>Gets the snapshot ot the move chosen as the current</summary>
    public BoardSnapshot CurrentSnapshot
    {
      get
      {
        if (CurrentMoveIndex == -1)
        {
          return Count > 0 ? this[0].BoardSnapshot : null;
        }
        if (CurrentMoveIndex < Count - 1)
        {
          return this[CurrentMoveIndex + 1].BoardSnapshot;
        }
        return CurrentMove.BoardSnapshot.MakeMove(CurrentMove.Move);
      }
    }

    /// <summary>Changes the current move to the last one</summary>
    public void GoToTheLast()
    {
      CurrentMoveIndex = Count - 1;
    }

    /// <summary>Gets move on the board parsing it from snapsot</summary>
    public DecoratedMove Decorate(Move snapshot)
    {
      // TODO: Using of this method is almost always ugly!
      if (snapshot == null) throw new ArgumentNullException("snapshot");
      return new DecoratedMove(snapshot);
    }
  }
}