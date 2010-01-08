using System;
using System.Diagnostics;
using System.Threading;

namespace Echo
{
  class Program
  {
    static void Main()
    {
      // Echo only works for 5 sec. then kills itself
      new Timer(state => Process.GetCurrentProcess().Kill(), null, 5000, 0);

      while (true)
      {
        string line = Console.ReadLine();
        if (line == null) return;
        if (line == "quit") return;
        if (line == "quit with delay")
        {
          Thread.Sleep(2000);
          return;
        }
        Console.WriteLine(line);
      }
    }
  }
}
