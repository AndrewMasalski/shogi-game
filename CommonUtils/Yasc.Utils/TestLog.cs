using System.Text;

namespace Yasc.Utils
{
  public class TestLog
  {
    readonly StringBuilder _sb = new StringBuilder();

    public void Write(string msg)
    {
      if (_sb.Length > 0) _sb.Append(" ");
      _sb.Append(msg ?? "<null>");
    }

    public override string ToString()
    {
      return _sb.ToString();
    }
  }
}