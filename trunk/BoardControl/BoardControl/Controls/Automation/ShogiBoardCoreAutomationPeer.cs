using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace Yasc.BoardControl.Controls.Automation
{
  public class ShogiBoardCoreAutomationPeer : ControlAutomationPeer<ShogiBoardCore>, IGridProvider
  {
    public ShogiBoardCoreAutomationPeer(ShogiBoardCore owner) 
      : base(owner)
    {
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      switch (patternInterface)
      {
        case PatternInterface.Grid:
          return this;
        default:
          return base.GetPattern(patternInterface);
      }
    }
    protected override string GetItemTypeCore()
    {
      return typeof(ShogiCell).Name;
    }
    protected override List<AutomationPeer> GetChildrenCore()
    {
      return (from p in Position.OnBoard 
              select CreatePeerForElement(Owner.GetCell(p))).ToList();
    }

    #region ' IGridProvider '


    IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
    {
      return ProviderFromPeer(CreatePeerForElement(Owner.GetCell(new Position(row, column))));
    }
    int IGridProvider.RowCount
    {
      get { return 9; }
    }
    int IGridProvider.ColumnCount
    {
      get { return 9; }
    }

    #endregion
  }
}