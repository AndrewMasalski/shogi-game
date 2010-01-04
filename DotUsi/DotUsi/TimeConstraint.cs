using System;
using System.Text;

namespace DotUsi
{
  public class TimeConstraint
  {
    public static readonly TimeConstraint InfiniteConstraint = new TimeConstraint { Infinite = true };

    /// <summary>Black has x milliseconds left on the clock</summary>
    /// <remarks>btime x</remarks>
    public TimeSpan BlackTime { get; private set; }
    /// <summary>White has x milliseconds left on the clock</summary>
    /// <remarks>wtime x</remarks>
    public TimeSpan WhiteTime { get; private set; }

    /// <summary>Black increment per move i milliseconds (if i > 0)</summary>
    /// <remarks>binc i</remarks>
    public TimeSpan BlackIncrement { get; private set; }
    /// <summary>White increment per move i milliseconds (if i > 0)</summary>
    /// <remarks>winc i</remarks>
    public TimeSpan WhiteIncrement { get; private set; }

    /// <summary>There are n moves to the next time control. 
    ///   This will only be sent if n > 0. 
    ///   If you don't get this and get the <see cref="WhiteTime"/> and <see cref="BlackTime"/>, it's sudden death.
    /// </summary>
    /// <remarks>movestogo n</remarks>
    public int MovesToGo { get; private set; }

    /// <summary>Search exactly t milliseconds</summary>
    /// <remarks>movetime t</remarks>
    public TimeSpan MoveTime { get; private set; }

    /// <summary>Search until the stop command is received. 
    ///   Do not exit the search without being told so in this mode!
    /// </summary>
    /// <remarks>infinite</remarks>
    public bool Infinite { get; set; }

    /// <summary>http://en.wikipedia.org/wiki/Byoyomi</summary>
    /// <remarks><para>byoyomi</para>
    /// <para>It's not from USI specification. Seen in Shogidokoro</para></remarks>
    public TimeSpan Byoyomi { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();

      if (WhiteTime != TimeSpan.Zero)
        sb.Append("wtime " + WhiteTime.TotalMilliseconds);
      if (BlackTime != TimeSpan.Zero)
        sb.Append("btime " + BlackTime.TotalMilliseconds);
      if (WhiteIncrement != TimeSpan.Zero)
        sb.Append("winc " + WhiteIncrement.TotalMilliseconds);
      if (BlackIncrement != TimeSpan.Zero)
        sb.Append("binc " + WhiteIncrement.TotalMilliseconds);
      if (Byoyomi != TimeSpan.Zero)
        sb.Append("byoyomi " + Byoyomi.TotalMilliseconds);
      if (MoveTime != TimeSpan.Zero)
        sb.Append("movetime " + MoveTime.TotalMilliseconds);
      if (MovesToGo != 0)
        sb.Append("movestogo " + MovesToGo);
      if (Infinite)
        sb.Append("infinite");

      return sb.ToString();
    }
  }
}