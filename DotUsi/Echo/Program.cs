using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Echo
{
  class Program
  {
    static void Main(string[] args)
    {
      while (true)
      {
        string line = Console.ReadLine();
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
