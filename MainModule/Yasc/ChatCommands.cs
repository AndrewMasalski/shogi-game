using System.Windows.Input;

namespace Yasc
{
  public static class ChatCommands
  {
    public static readonly RoutedUICommand SendMessage
         = new RoutedUICommand("SendMessage", "SendMessage", typeof(ChatCommands));
  }
}