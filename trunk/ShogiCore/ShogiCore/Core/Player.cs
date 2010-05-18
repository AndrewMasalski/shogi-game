namespace Yasc.ShogiCore
{
  /// <summary>Game participiant</summary>
  public class Player
  {
    /// <summary>Board game is going on</summary>
    public Board Board { get; private set; }
    /// <summary>The player name</summary>
    public string Name { get; set; }
    /// <summary>The pieces player has in hand</summary>
    public Hand Hand { get; internal set; }

    internal Player(Board board)
    {
      Board = board;
    }

    /// <summary>The player opponent</summary>
    public Player Opponent
    {
      get { return Board.Black == this ? Board.White : Board.Black; }
    }
    /// <summary>The player color</summary>
    public PieceColor Color
    {
      get { return Board.White == this ? PieceColor.White : PieceColor.Black; }
    }
    /// <summary>Get human readable representation of the player</summary>
    public override string ToString()
    {
      return Name ?? Color.ToString();
    }
  }
}