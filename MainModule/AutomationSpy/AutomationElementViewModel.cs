using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Threading;
using MvvmFoundation.Wpf;
using Yasc.Utils;

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
    public ObservableCollection<ElementPropertyViewModel> Properties { get; private set; }
    public ObservableCollection<AutomationElementViewModel> Children { get; private set; }

    public AutomationElementViewModel(AutomationElement element, Condition filterChildrenCondition)
    {
      Element = element;
      Children = new ObservableCollection<AutomationElementViewModel>();
      Properties = new ObservableCollection<ElementPropertyViewModel>();
      _filterChildrenCondition = filterChildrenCondition;
      _disp = Dispatcher.CurrentDispatcher;
    }

    #region ' Refresh '

    public void Refresh()
    {
      // This is called on timer thread!
      Caption = GetCaption();

      if (IsSelected)
        RefreshProperties();

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
        Cast<AutomationElement>();

      _disp.Invoke(
        DispatcherPriority.Background,
        new Action(() => UpdateChildren(children)));
    }
    private void UpdateChildren(IEnumerable<AutomationElement> children)
    {
      // This is called on timer thread!
      Children.Update(children, evm => evm.Element, ae => ae,
        ae => new AutomationElementViewModel(ae, Condition.TrueCondition));
    }
    private void RefreshProperties()
    {
      // This is called on timer thread!
      if (Properties.Count == 0)
      {
        var properties =
          (from p in Element.GetSupportedProperties()
           select new ElementPropertyViewModel(Element, p)).ToList();

        _disp.BeginInvoke(DispatcherPriority.Background, new Action(() =>
          {
            foreach (var p in properties)
              Properties.Add(p);
          }));
      }
      else
      {
        foreach (var p in Properties)
          p.Refresh();
      }
    }
    private string GetCaption()
    {
      // This is called on timer thread!
      var name = Element.Current.Name; //(string)Element.GetCurrentPropertyValue(AutomationElement.NameProperty);
      if (!string.IsNullOrEmpty(name)) return name;

      var className = Element.Current.ClassName; //(string)Element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
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