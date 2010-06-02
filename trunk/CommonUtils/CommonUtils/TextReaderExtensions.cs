using System;
using System.Collections.Generic;
using System.IO;

namespace Yasc.Utils
{
  public static class TextReaderExtensions
  {
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
        if (line == null) break;
        yield return line;
      }
    }
  }
}