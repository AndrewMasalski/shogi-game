using System.Windows;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ControlAutomationPeer<T> : FrameworkElementAutomationPeer
    where T : FrameworkElement
  {
    public ControlAutomationPeer(T owner) 
      : base(owner)
    {
    }
    public new T Owner
    {
      get { return (T)base.Owner; }
    }
    protected override string GetClassNameCore()
    {
      return Owner.GetType().Name;
    }
  }
}