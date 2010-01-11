using System;

namespace Yasc.Gui
{
  public class ChatMessage
  {
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    public string Owner { get; set; }

    public ChatMessage(DateTime timestamp, string message, string owner)
    {
      Timestamp = timestamp;
      Message = message;
      Owner = owner;
    }
  }
}