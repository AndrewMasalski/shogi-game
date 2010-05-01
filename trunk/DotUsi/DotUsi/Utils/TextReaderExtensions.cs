using System;
using System.Collections.Generic;
using System.IO;

namespace DotUsi.Utils
{
  internal static class TextReaderExtensions
  {
    public static IEnumerable<string> LinesUntil(this TextReader reader, string stopLine)
    {
      if (reader == null) throw new ArgumentNullException("reader");
      if (stopLine == null) throw new ArgumentNullException("stopLine");
      return LinesUntilInternal(reader, stopLine);
    }
    private static IEnumerable<string> LinesUntilInternal(TextReader reader, string stopLine)
    {
      while (true)
      {
        var line = reader.ReadLine();
        if (line == null || line == stopLine)
          break;
        yield return line;
      }
    }/*
    public static IEnumerable<string> Lines(this TextReader reader)
    {
      if (reader == null) throw new ArgumentNullException("reader");
      return LinesInternal(reader);
    }
    private static IEnumerable<string> LinesInternal(TextReader reader)
    {
      while (true)
      {
        var line = reader.ReadLine();
        if (line == null)
          break;
        yield return line;
      }
    }*/
  }
}