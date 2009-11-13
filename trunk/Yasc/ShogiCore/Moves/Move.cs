using System;

namespace Yasc.ShogiCore.Moves
{
  public abstract class MoveBase
  {
    public Board Board { get; private set; }
    public DateTime TimeStamp { get; private set; }
    public Player Who { get; private set; }
    public int Number { get; private set; }
    public BoardSnapshot BoardSnapshot { get; private set; }
    public bool IsValid { get { return ErrorMessage == null; }}
    public string ErrorMessage { get; private set; }

    protected MoveBase(Board board, Player who)
    {
      if (board == null) throw new ArgumentNullException("board");
      
      Board = board;
      TimeStamp = DateTime.Now;
      Who = who;
      Number = Board.History.MovesDone.Count + 1;
      BoardSnapshot = Board.CurrentSnapshot;
    }


    protected internal abstract void Make();
    
    protected void Validate()
    {
      ErrorMessage = GetErrorMessage();
    }

    protected abstract string GetErrorMessage();
  }
}