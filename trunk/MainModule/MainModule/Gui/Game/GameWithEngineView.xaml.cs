using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Yasc.BoardControl.Controls;

namespace MainModule.Gui.Game
{
  public partial class GameWithEngineView
  {
    public GameWithEngineView()
    {
      InitializeComponent();
      Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
      Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
        new Action(CommandManager.InvalidateRequerySuggested));
    }

    private void OnMoveAttempt(object sender, MoveAttemptEventArgs e)
    {
      Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
        new Action(CommandManager.InvalidateRequerySuggested));

      _errorLabel.Text = e.Move.RulesViolation.ToString();
    }
  }
}