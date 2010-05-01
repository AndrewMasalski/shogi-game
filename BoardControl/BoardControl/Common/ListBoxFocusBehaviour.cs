using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace Yasc.BoardControl.Common
{
  public class ListBoxFocusBehaviour
  {
    private readonly ListBox _listBox;

    public ListBoxFocusBehaviour(ListBox listBox)
    {
      if (listBox == null) throw new ArgumentNullException("listBox");
      _listBox = listBox;
      _listBox.PreviewKeyDown += OnPreviewKeyDown;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Left && e.Key != Key.Right) return;
      FocusTheNearestFocusableItem(_listBox.SelectedIndex, e.Key == Key.Left ? -1 : +1);
      e.Handled = true;
    }

    #region ' FocusTheNearestFocusableItem '

    private void FocusTheNearestFocusableItem(int startIndex, int searchStep)
    {
      Debug.Assert(searchStep != 0);
      startIndex += searchStep;
      if (searchStep < 0)
      {
        SearchForward(searchStep, startIndex);
      }
      else
      {
        SearchBackwards(searchStep, startIndex);
      }
    }
    private void SearchBackwards(int searchStep, int startIndex)
    {
      int count = _listBox.Items.Count;
      for (int i = startIndex; i < count; i += searchStep)
      {
        var item = (ListBoxItem)_listBox.ItemContainerGenerator.ContainerFromIndex(i);
        if (item.Focusable)
        {
          item.Focus();
          return;
        }
      }
    }
    private void SearchForward(int searchStep, int startIndex)
    {
      for (int i = startIndex; i >= 0; i += searchStep)
      {
        var item = (ListBoxItem)_listBox.ItemContainerGenerator.ContainerFromIndex(i);
        if (item.Focusable)
        {
          item.Focus();
          return;
        }
      }
    }

    #endregion
  }
}