﻿using System.Windows.Input;
using Yasc.BoardControl.Controls;

namespace MainModule.Gui.Game
{
  public partial class GameView
  {
    public GameView()
    {
      InitializeComponent();
    }

    private void OnMoveAttempt(object sender, MoveAttemptEventArgs e)
    {
      _errorLabel.Text = e.Move.RulesViolation.ToString();
    }

    private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var vm = DataContext as GameWithOpponentViewModel;
      if (vm != null) vm.SendMessageCommand.Execute(null);
    }
  }
}