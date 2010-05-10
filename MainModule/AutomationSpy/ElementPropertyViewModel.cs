using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using Yasc.Utils.Mvvm;

namespace AutomationSpy
{
  public class ElementPropertyViewModel : ObservableObject
  {
    private string _value;

    public string Name
    {
      get
      {
        return Property.ProgrammaticName.
          Split('.').Last().Replace("Property", "");
      }
    }
    public string Value
    {
      get { return _value; }
      set
      {
        if (_value == value) return;
        _value = value;
        RaisePropertyChanged("Value");
      }
    }

    private static readonly Dictionary<int, string> _categories;

    static ElementPropertyViewModel()
    {
      _categories = new Dictionary<int, string>
        {
          { AutomationElementIdentifiers.AcceleratorKeyProperty.Id, "Misc"},
          { AutomationElementIdentifiers.AccessKeyProperty.Id, "Misc"},
          { AutomationElementIdentifiers.AutomationIdProperty.Id, "ID"},
          { AutomationElementIdentifiers.BoundingRectangleProperty.Id, "Misc"},
          { AutomationElementIdentifiers.ClassNameProperty.Id, "ID"},
          { AutomationElementIdentifiers.ClickablePointProperty.Id, "Misc"},
          { AutomationElementIdentifiers.ControlTypeProperty.Id, "Type"},
          { AutomationElementIdentifiers.CultureProperty.Id, "Misc"},
          { AutomationElementIdentifiers.FrameworkIdProperty.Id, "Type"},
          { AutomationElementIdentifiers.HasKeyboardFocusProperty.Id, "Misc"},
          { AutomationElementIdentifiers.HelpTextProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsContentElementProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsControlElementProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsDockPatternAvailableProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsEnabledProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsGridItemPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsGridPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsInvokePatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsItemContainerPatternAvailableProperty.Id, "MHiddenisc"},
          { AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsOffscreenProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsPasswordProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsRequiredForFormProperty.Id, "Misc"},
          { AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsScrollPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsSynchronizedInputPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsTableItemPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsTablePatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsTextPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsTogglePatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsTransformPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsValuePatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsVirtualizedItemPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.IsWindowPatternAvailableProperty.Id, "Hidden"},
          { AutomationElementIdentifiers.ItemStatusProperty.Id, "Misc"},
          { AutomationElementIdentifiers.ItemTypeProperty.Id, "Type"},
          { AutomationElementIdentifiers.LabeledByProperty.Id, "ID"},
          { AutomationElementIdentifiers.LocalizedControlTypeProperty.Id, "Type"},
          { AutomationElementIdentifiers.NameProperty.Id, "ID"},
          { AutomationElementIdentifiers.NativeWindowHandleProperty.Id, "Tech"},
          { AutomationElementIdentifiers.OrientationProperty.Id, "Misc"},
          { AutomationElementIdentifiers.ProcessIdProperty.Id, "Tech"},
          { AutomationElementIdentifiers.RuntimeIdProperty.Id, "Tech"},

          { DockPatternIdentifiers.DockPositionProperty.Id, "Dock"},
          { ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id, "ExpandCollapse"},
          { GridItemPatternIdentifiers.ColumnProperty.Id, "GridItem"},
          { GridItemPatternIdentifiers.ColumnSpanProperty.Id, "GridItem"},
          { GridItemPatternIdentifiers.ContainingGridProperty.Id, "GridItem"},
          { GridItemPatternIdentifiers.RowProperty.Id, "GridItem"},
          { GridItemPatternIdentifiers.RowSpanProperty.Id, "GridItem"},
          { GridPatternIdentifiers.ColumnCountProperty.Id, "Grid"},
          { GridPatternIdentifiers.RowCountProperty.Id, "Grid"},
          { MultipleViewPatternIdentifiers.CurrentViewProperty.Id, "MultipleView"},
          { MultipleViewPatternIdentifiers.SupportedViewsProperty.Id, "MultipleView"},
          { RangeValuePatternIdentifiers.IsReadOnlyProperty.Id, "RangeValue"},
          { RangeValuePatternIdentifiers.LargeChangeProperty.Id, "RangeValue"},
          { RangeValuePatternIdentifiers.MaximumProperty.Id, "RangeValue"},
          { RangeValuePatternIdentifiers.MinimumProperty.Id, "RangeValue"},
          { RangeValuePatternIdentifiers.SmallChangeProperty.Id, "RangeValue"},
          { RangeValuePatternIdentifiers.ValueProperty.Id, "RangeValue"},
          { ScrollPatternIdentifiers.HorizontallyScrollableProperty.Id, "Scroll"},
          { ScrollPatternIdentifiers.HorizontalScrollPercentProperty.Id, "Scroll"},
          { ScrollPatternIdentifiers.HorizontalViewSizeProperty.Id, "Scroll"},
          { ScrollPatternIdentifiers.VerticallyScrollableProperty.Id, "Scroll"},
          { ScrollPatternIdentifiers.VerticalScrollPercentProperty.Id, "Scroll"},
          { ScrollPatternIdentifiers.VerticalViewSizeProperty.Id, "Scroll"},
          { SelectionItemPatternIdentifiers.IsSelectedProperty.Id, "SelectionItem"},
          { SelectionItemPatternIdentifiers.SelectionContainerProperty.Id, "SelectionItem"},
          { SelectionPatternIdentifiers.CanSelectMultipleProperty.Id, "Selection"},
          { SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id, "Selection"},
          { SelectionPatternIdentifiers.SelectionProperty.Id, "Selection"},
          { TableItemPatternIdentifiers.ColumnHeaderItemsProperty.Id, "TableItem"},
          { TableItemPatternIdentifiers.RowHeaderItemsProperty.Id, "TableItem"},
          { TablePatternIdentifiers.ColumnHeadersProperty.Id, "Table"},
          { TablePatternIdentifiers.RowHeadersProperty.Id, "Table"},
          { TablePatternIdentifiers.RowOrColumnMajorProperty.Id, "Table"},
          { TogglePatternIdentifiers.ToggleStateProperty.Id, "Toggle"},
          { TransformPatternIdentifiers.CanMoveProperty.Id, "Transform"},
          { TransformPatternIdentifiers.CanResizeProperty.Id, "Transform"},
          { TransformPatternIdentifiers.CanRotateProperty.Id, "Transform"},
          { ValuePatternIdentifiers.IsReadOnlyProperty.Id, "Value"},
          { ValuePatternIdentifiers.ValueProperty.Id, "Value"},
          { WindowPatternIdentifiers.CanMaximizeProperty.Id, "Window"},
          { WindowPatternIdentifiers.CanMinimizeProperty.Id, "Window"},
          { WindowPatternIdentifiers.IsModalProperty.Id, "Window"},
          { WindowPatternIdentifiers.IsTopmostProperty.Id, "Window"},
          { WindowPatternIdentifiers.WindowInteractionStateProperty.Id, "Window"},
          { WindowPatternIdentifiers.WindowVisualStateProperty.Id, "Window"},        
        };
    }
    
    public string Category
    {
      get { return _categories[Property.Id]; }
    }
    public int CategoryRank
    {
      get
      {
        switch (Category)
        {
          case "ID": return 0;

          case "Misc": return 1;
          case "Type": return 2;

          case "Dock": return 25;
          case "ExpandCollapse": return 25;
          case "GridItem": return 25;
          case "Grid": return 25;
          case "MultipleView": return 25;
          case "RangeValue": return 25;
          case "Scroll": return 25;
          case "SelectionItem": return 25;
          case "Selection": return 25;
          case "TableItem": return 25;
          case "Table": return 25;
          case "Toggle": return 25;
          case "Transform": return 25;
          case "Value": return 25;
          case "Window": return 25;

          case "Tech": return 49;
          default: return 50;
          case "Hidden": return 100;
        }
      }
    }
    public AutomationElement Element { get; set; }
    public AutomationProperty Property { get; set; }

    public ElementPropertyViewModel(AutomationElement element, AutomationProperty property)
    {
      Element = element;
      Property = property;
      Value = GetValue();
    }

    public void Refresh()
    {
      // This is called on timer thread!
      Value = GetValue();
    }
    private string GetValue()
    {
      // This is called on timer thread!
      var value = Element.GetCurrentPropertyValue(Property);
      var str = value as string;
      if (str != null)
      {
        return str == "" ? "<empty>" : str;
      }
      var list = value as IEnumerable;
      if (list != null)
      {
        return "(" + string.Join(", ", list.Cast<object>().Select(o => o.ToString()).ToArray()) + ")";
      }
      var controlType = value as ControlType;
      if (controlType != null)
      {
        return controlType.ProgrammaticName.Split('.').Last();
      }
      return value != null ? value.ToString() : "<null>";
    }
  }
}