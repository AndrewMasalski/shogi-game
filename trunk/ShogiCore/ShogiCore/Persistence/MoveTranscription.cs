using System;
using System.Text;

namespace Yasc.ShogiCore.Persistence
{
  public class MoveTranscription
  {
    public int Number { get; set; }
    public string MoveNotation { get; set; }
    public string Comment { get; set; }
    public string Evaluation { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();
      if (Number != null)
      {
        sb.Append(Number);
        sb.Append(". ");
      }
      sb.Append(MoveNotation);
      if (!string.IsNullOrWhiteSpace(Comment))
      {
        sb.Append(" // ");
        sb.Append(Comment);
      }
      return sb.ToString();
    }
  }
}