using System.Windows;
using System.Windows.Input;
using Yasc.Controls;

namespace Yasc.Gui.Game
{
  public partial class GameWithEngineView
  {
    public GameWithEngineView()
    {
      InitializeComponent();
    }

    private void OnMoveAttempt(object sender, MoveAttemptEventArgs e)
    {
      _errorLabel.Text = e.Move.ErrorMessage;
    }

    private void OnUndo(object sender, ExecutedRoutedEventArgs e)
    {
      var vm = DataContext as GameWithOpponentViewModel;
      if (vm != null) vm.UndoLastMove();
    }

    private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
      e.Handled = true;
    }
  }
}