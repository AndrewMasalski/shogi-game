using System;

namespace TestStand.WelcomeView
{
  public class StandEvent
  {
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }

    public StandEvent(DateTime timestamp, string message)
    {
      Timestamp = timestamp;
      Message = message;
    }

    public StandEvent(string message)
    {
      Timestamp = DateTime.Now;
      Message = message;
    }
  }
}