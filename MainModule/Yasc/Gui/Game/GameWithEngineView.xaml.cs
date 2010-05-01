using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Yasc.BoardControl.Controls;

namespace Yasc.Gui.Game
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

      _errorLabel.Text = e.Move.ErrorMessage;
    }

//    private void OnUndo(object sender, ExecutedRoutedEventArgs e)
//    {
//      var vm = DataContext as GameWithOpponentViewModel;
//      if (vm != null) vm.UndoLastMove();
//    }

    private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
      e.Handled = true;
    }

  }
}