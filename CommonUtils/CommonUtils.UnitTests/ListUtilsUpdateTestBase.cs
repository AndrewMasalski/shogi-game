using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CommonUtils.UnitTests
{
  public abstract class ListUtilsUpdateTestBase
  {
    protected static string ToString(NotifyCollectionChangedEventArgs args)
    {
      var sb = new StringBuilder(args.Action.ToString());
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          sb.Append("(");
          sb.Append(args.NewStartingIndex);
          sb.Append(", ");
          sb.Append(Join(args.NewItems));
          sb.Append(")");
          break;
        case NotifyCollectionChangedAction.Remove:
          sb.Append("(");
          sb.Append(args.OldStartingIndex);
          sb.Append(", ");
          sb.Append(Join(args.OldItems));
          sb.Append(")");
          break;
        case NotifyCollectionChangedAction.Replace:
          sb.Append("(");
          sb.Append(args.OldStartingIndex);
          sb.Append(", ");
          sb.Append(Join(args.NewItems));
          sb.Append(")");
          break;
        case NotifyCollectionChangedAction.Move:
          sb.Append("(");
          sb.Append(args.OldStartingIndex);
          sb.Append("<->");
          sb.Append(args.NewStartingIndex);
          sb.Append(", ");
          sb.Append(Join(args.NewItems));
          sb.Append(")");
          break;
        case NotifyCollectionChangedAction.Reset:
          break;
        default:
          throw new NotSupportedException();
      }
      return sb.ToString();
    }

    protected static string Join(IEnumerable list)
    {
      var arr = list.Cast<object>().Select(o => o.ToString()).ToArray();
      return "[" + string.Join(", ", arr) + "]";
    }
    
  }
}