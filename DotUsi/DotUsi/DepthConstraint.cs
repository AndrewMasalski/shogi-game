using System.Text;

namespace DotUsi
{
  public class DepthConstraint
  {
    /// <summary>Search x plies only.</summary>
    /// <remarks>depth x</remarks>
    public int Depth { get; set; }

    /// <summary>Search x nodes only.</summary>
    /// <remarks>nodes x</remarks>
    public int Nodes { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();

      if (Depth != 0)
        sb.Append("depth " + Depth);
      if (Nodes != 0)
        sb.Append("nodes " + Nodes);

      return sb.ToString();
    }
  }
}