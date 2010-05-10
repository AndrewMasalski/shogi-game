using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Data;
using System.Windows.Threading;
using Yasc.Utils;
using Yasc.Utils.Mvvm;

namespace AutomationSpy
{
  public class AutomationElementViewModel : ObservableObject
  {
    #region ' Fields '

    private bool _isExpanded;
    private bool _isSelected;
    private string _caption;
    private readonly Condition _filterChildrenCondition;
    private readonly Dispatcher _disp;
    private static readonly Dictionary<int, string> _verbs;
    private readonly ObservableCollection<ElementPropertyViewModel> _properties;

    #endregion

    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected == value) return;
        _isSelected = value;
        RaisePropertyChanged("IsSelected");
      }
    }
    public bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        if (_isExpanded == value) return;
        _isExpanded = value;
        RaisePropertyChanged("IsExpanded");
      }
    }
    public string Caption
    {
      get { return _caption; }
      private set
      {
        if (_caption == value) return;
        _caption = value;
        RaisePropertyChanged("Caption");
      }
    }
    public AutomationElement Element { get; private set; }
    public ICollectionView Properties { get; private set; }
    public ObservableCollection<AutomationPatternViewModel> Patterns { get; private set; }
    public ObservableCollection<AutomationElementViewModel> Children { get; private set; }
    public string VerbalFlags
    {
      get
      {
        var list = new List<string>();
        foreach (var automationProperty in Element.GetSupportedProperties())
        {
          string verb;
          if (!_verbs.TryGetValue(automationProperty.Id, out verb)) continue;
          verb = verb.ToLower();
          var flag = (bool)Element.GetCurrentPropertyValue(automationProperty);
          if (!flag)
            verb = verb.Replace("is", "is not").
              Replace("can", "can't").
              Replace("has", "hasn't");
          list.Add(verb);
        }
        return string.Join(", ", list);
      }
    }

    static AutomationElementViewModel()
    {
      _verbs = new Dictionary<int, string>
        {
          { AutomationElementIdentifiers.HasKeyboardFocusProperty.Id, "Has keyboard focus"},
          { AutomationElementIdentifiers.IsContentElementProperty.Id, "Is content element"},
          { AutomationElementIdentifiers.IsControlElementProperty.Id, "Is control element"},
          { AutomationElementIdentifiers.IsEnabledProperty.Id, "Is enabled"},
          { AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id, "Is keyboard focusable"},
          { AutomationElementIdentifiers.IsOffscreenProperty.Id, "Is offscreen"},
          { AutomationElementIdentifiers.IsPasswordProperty.Id, "Is password"},
          { AutomationElementIdentifiers.IsRequiredForFormProperty.Id, "Is required for form"},

          { RangeValuePatternIdentifiers.IsReadOnlyProperty.Id, "Is readonly"},
          { SelectionItemPatternIdentifiers.IsSelectedProperty.Id, "Is selected"},
          { SelectionPatternIdentifiers.CanSelectMultipleProperty.Id, "Can select multiple"},
          { SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id, "Selection is required"},
          { TransformPatternIdentifiers.CanMoveProperty.Id, "Can move"},
          { TransformPatternIdentifiers.CanResizeProperty.Id, "Can resize"},
          { TransformPatternIdentifiers.CanRotateProperty.Id, "Can rotate"},
          { ValuePatternIdentifiers.IsReadOnlyProperty.Id, "Is readonly"},
          { WindowPatternIdentifiers.CanMaximizeProperty.Id, "Can maximize"},
          { WindowPatternIdentifiers.CanMinimizeProperty.Id, "Can minimize"},
          { WindowPatternIdentifiers.IsModalProperty.Id, "Is modal"},
          { WindowPatternIdentifiers.IsTopmostProperty.Id, "Is topmost"},                   
          
          { ScrollPatternIdentifiers.HorizontallyScrollableProperty.Id, "is horizontally scrollable"},
          { ScrollPatternIdentifiers.VerticallyScrollableProperty.Id, "is vertically scrollable"},
         };
    }

    public AutomationElementViewModel(AutomationElement element, Condition filterChildrenCondition)
    {
      Element = element;
      Children = new ObservableCollection<AutomationElementViewModel>();
      _properties = new ObservableCollection<ElementPropertyViewModel>();
      var collectionView = new CollectionViewSource {Source = _properties};
      collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
      collectionView.SortDescriptions.Add(new SortDescription("CategoryRank", ListSortDirection.Ascending));
      Properties = collectionView.View;
      Patterns = new ObservableCollection<AutomationPatternViewModel>();
      _filterChildrenCondition = filterChildrenCondition;
      _disp = Dispatcher.CurrentDispatcher;
    }

    public void WalkExpandedElements(Action<AutomationElementViewModel> callback)
    {
      callback(this);

      // Getting locked copy of children collection
      IEnumerable<AutomationElementViewModel> children;
      lock (Children)
        children = Children.ToList();

      foreach (var element in children)
      {
        if (element.IsExpanded)
        {
          element.WalkExpandedElements(callback);
        }
        else
        {
          callback(element);
        }
      }
    }

    #region ' Refresh '

    public void Refresh()
    {
      // This is called on timer thread!
      Caption = GetCaption();

      if (IsSelected)
      {
        RefreshProperties();
        RefreshPatterns();
      }

      if (!IsExpanded)
        foreach (var child in Children)
          child.ResetChildrenCache();

      RefreshChildren();
    }

    private void RefreshChildren()
    {
      // This is called on timer thread!
      var children = Element.FindAll(
        TreeScope.Children, _filterChildrenCondition).
        Cast<AutomationElement>().Distinct();

      _disp.Invoke(
        DispatcherPriority.Background,
        new Action(() => UpdateChildren(children)));
    }
    private void UpdateChildren(IEnumerable<AutomationElement> children)
    {
      // This is called on timer thread!
      lock (Children)
        Children.Update(children, evm => evm.Element, ae => ae,
          ae => new AutomationElementViewModel(ae, Condition.TrueCondition));
    }
    private void RefreshProperties()
    {
      // This is called on timer thread!
      if (_properties.Count == 0)
      {
        var properties =
          (from p in Element.GetSupportedProperties()
           where !_verbs.ContainsKey(p.Id)
           select new ElementPropertyViewModel(Element, p)).ToList();

        _disp.BeginInvoke(DispatcherPriority.Background, new Action(() =>
          {
            foreach (var p in properties)
              _properties.Add(p);
          }));
      }
      else
      {
        foreach (var p in _properties)
          p.Refresh();
      }
    }
    private void RefreshPatterns()
    {
      // This is called on timer thread!
      if (Patterns.Count == 0)
      {
        var patterns =
          (from p in Element.GetSupportedPatterns()
           select new AutomationPatternViewModel(Element, p)).ToList();

        _disp.BeginInvoke(DispatcherPriority.Background, new Action(() =>
          {
            foreach (var p in patterns)
              Patterns.Add(p);
          }));
      }
    }
    private string GetCaption()
    {
      // This is called on timer thread!
      var name = Element.Current.Name;
      if (!string.IsNullOrEmpty(name)) return name;

      var className = Element.Current.ClassName;
      if (!string.IsNullOrEmpty(className)) return className;

      var localizedControlType = Element.Current.LocalizedControlType;
      if (!string.IsNullOrEmpty(localizedControlType)) return localizedControlType;

      return "???";
    }
    private void ResetChildrenCache()
    {
      // This is called on timer thread!
      _disp.BeginInvoke(DispatcherPriority.Background, new Action(() => Children.Clear()));
    }

    #endregion
  }
}