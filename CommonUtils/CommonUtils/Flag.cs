using System;

namespace Yasc.Utils
{
  public class Flag
  {
    private uint _counter;
    public IDisposable Set()
    {
      _counter++;
      return new Substractor(this);
    }

    private struct Substractor : IDisposable
    {
      private readonly Flag _flag;
      public Substractor(Flag flag)
      {
        _flag = flag;
      }

      public void Dispose()
      {
        checked { _flag._counter--; }
      }
    }

    public static implicit operator bool(Flag f)
    {
      return f._counter > 0;
    }
  }
}