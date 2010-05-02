using System.Windows.Input;

namespace MainModule
{
  public static class ChatCommands
  {
    public static readonly RoutedUICommand SendMessage
         = new RoutedUICommand("SendMessage", "SendMessage", typeof(ChatCommands));
  }
//  public static class GameCommands
//  {
//    public static readonly RoutedUICommand TakeBack
//         = new RoutedUICommand("TakeBack", "TakeBack", typeof(GameCommands));
//  }
}