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

    private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ((GameWithEngineViewModel)DataContext).SendMessageCommand.Execute(null);
    }
  }
}