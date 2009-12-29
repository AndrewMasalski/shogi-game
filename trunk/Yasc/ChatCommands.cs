using System.Windows.Input;

namespace Yasc
{
  public class ChatCommands
  {
    public static RoutedUICommand SendMessage
         = new RoutedUICommand("SendMessage", "SendMessage", typeof(ChatCommands));
  }
}