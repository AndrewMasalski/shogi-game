﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yasc.Controls;

namespace Yasc.Gui
{
  public partial class GameView
  {
    public GameView()
    {
      InitializeComponent();
    }

    private void OnMoveAttempt(object sender, MoveAttemptEventArgs e)
    {
      _errorLabel.Text = e.Move.ErrorMessage;
    }

    internal class FocusCheater
    {
      private readonly ListBox _listBox;

      public FocusCheater(ListBox listBox)
      {
        if (listBox == null) throw new ArgumentNullException("listBox");
        _listBox = listBox;
        _listBox.PreviewKeyDown += OnPreviewKeyDown;
      }

      private void OnPreviewKeyDown(object sender, KeyEventArgs e)
      {
        int index = _listBox.SelectedIndex;
        if (e.Key == Key.Left)
        {
          if (index > 0 && index % 2 == 0)
          {
            var lbi = (ListBoxItem)_listBox.ItemContainerGenerator.ContainerFromIndex(index - 1);
            lbi.Focus();
            e.Handled = true;
          }
        }
        else if (e.Key == Key.Right)
        {
          if (index < _listBox.Items.Count - 1 && index % 2 == 1)
          {
            var lbi = (ListBoxItem)_listBox.ItemContainerGenerator.ContainerFromIndex(index + 1);
            lbi.Focus();
            e.Handled = true;
          }
          else if (index == _listBox.Items.Count - 1)
          {
            e.Handled = true;
          }
        }
      }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      new FocusCheater(_listBox);
    }
  }
}