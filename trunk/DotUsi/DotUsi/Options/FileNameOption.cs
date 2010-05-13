using System.IO;
using Yasc.DotUsi.Options.Base;

namespace Yasc.DotUsi.Options
{
  /// <summary>Represents option of type <see cref="UsiOptionType.FileName"/></summary>
  public class FileNameOption : ValueOptionBase<FileInfo>
  {
    internal FileNameOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.FileName, new FileInfo(defaultValue))
    {
    }

    /// <summary>Override to specify how the <see cref="ValueOptionBase{T}.Value"/> should be represented in USI command</summary>
    protected override string ValueToString()
    {
      return Value.FullName;
    }
  }
}