using System;
using System.Windows.Threading;

namespace UnitTests.Netwroking
{
  public static class DispatcherUtils
  {
    public static void WaitForAllDefferedOperations()
    {
      Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(() => 1.ToString()));
    }    
  }
}