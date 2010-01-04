using System;

namespace DotUsi
{
  public class UsiParserError : Exception
  {
    public UsiParserError()
    {
    }

    public UsiParserError(string message)
      : base(message)
    {
    }
  }
}