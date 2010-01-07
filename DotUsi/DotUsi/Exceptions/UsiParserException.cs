using System;

namespace DotUsi
{
  /// <summary>Represents error that occurs during parse of engine's output</summary>
  public class UsiParserException : Exception
  {
    /// <summary>ctor</summary>
    internal UsiParserException()
    {
    }

    /// <summary>ctor</summary>
    internal UsiParserException(string message)
      : base(message)
    {
    }
  }
}