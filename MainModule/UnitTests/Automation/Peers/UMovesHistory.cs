using System.Collections.Generic;
using System.Windows.Automation;
using White.Core.UIItems;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;
using White.Core.UIItems.Finders;
using White.Core.UIItems.ListBoxItems;
using System.Linq;
using White.Core.UIItems.TabItems;

namespace MainModule.UnitTests.Automation.Peers
{
  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UMovesHistory : TabPage
  {
    protected UMovesHistory(AutomationElement automationElement, ActionListener actionListener) 
      : base(automationElement, actionListener)
    {

    }

    protected UMovesHistory()
    {
    }

//    public IEnumerable<ListItem> Records
//    {
//      get
//      {
//        return Container.GetMultiple(
          //        SearchCriteria.Indexed(0)
//          SearchCriteria.All
//          SearchCriteria.ByControlType(ControlType.ListItem)
//          ).Cast<ListItem>();
//      }
//    }
  }
}