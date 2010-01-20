using System.Windows;
using Yasc.Common;

namespace Yasc.Gui.Game
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
