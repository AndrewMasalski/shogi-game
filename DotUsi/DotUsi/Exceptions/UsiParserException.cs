using System;

namespace DotUsi
{
  public class UsiParserException : Exception
  {
    public UsiParserException()
    {
    }

    public UsiParserException(string message)
      : base(message)
    {
    }
  }
}