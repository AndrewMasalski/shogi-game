using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yasc.Common;
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

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      new ListBoxFocusBehaviour(_listBox);
    }
  }
}
