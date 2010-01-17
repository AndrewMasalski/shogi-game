using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Automation;
using MvvmFoundation.Wpf;
using Yasc.Utils;

namespace AutomationSpy
{
  public class AutomationElementViewModel : ObservableObject
  {
    private bool _isExpanded;
    private bool _isSelected;
    private ObservableCollection<ElementPropertyViewModel> _properties;
    private ObservableCollection<AutomationElementViewModel> _children;

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

    public AutomationElement Element { get; private set; }
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

    public void ResetChildrenCache()
    {
      _children = null;
    }
    
    public string Caption
    {
      get
      {
        return !string.IsNullOrEmpty(Element.Current.Name) ?
          Element.Current.Name : Element.Current.ClassName;
      }
    }

    public AutomationElementViewModel(AutomationElement element)
    {
      Element = element;
    }

    public ObservableCollection<ElementPropertyViewModel> Properties
    {
      get
      {
        if (_properties == null)
        {
          _properties = new ObservableCollection<ElementPropertyViewModel>(
            from p in Element.GetSupportedProperties()
            select new ElementPropertyViewModel(Element, p));
        }
        return _properties;
      }
    }
    public ObservableCollection<AutomationElementViewModel> Children
    {
      get
      {
        if (_children == null)
        {
          _children = new ObservableCollection<AutomationElementViewModel>(GetChildren());
        }
        return _children;
      }
    }

    private IEnumerable<AutomationElementViewModel> GetChildren()
    {
      return from r in Element.FindAll(TreeScope.Children, Condition.TrueCondition).Cast<AutomationElement>()
             select new AutomationElementViewModel(r);
    }

    public void RefreshChildren()
    {
      _children.Update(GetChildren(), (x, y) => x.Element == y.Element, x => x);
    }
  }

  public static class A
  {
    public static void Update<TDest, TSrc>(this ObservableCollection<TDest> dest, IEnumerable<TSrc> src, Comparer<TDest, TSrc> comparer, Converter<TSrc, TDest> converter)
    {

    }
  }
}