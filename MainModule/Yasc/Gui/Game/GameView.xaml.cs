using System.Windows;
using System.Windows.Input;
using Yasc.Common;
using Yasc.Controls;

namespace Yasc.Gui.Game
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

    private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ((GameWithOpponentViewModel)DataContext).SendMessageCommand.Execute(null);
    }
  }
}