using System.Windows;
using Yasc.BoardControl.Common;

namespace MainModule.Gui.Game
{
  public partial class MovesAndCommentsControl
  {
    public MovesAndCommentsControl()
    {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      new ListBoxFocusBehaviour(_listBox);
    }
  }
}
