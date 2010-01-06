using System.IO;

namespace DotUsi
{
  public class FileNameOption : ValueOptionBase<FileInfo>
  {
    public FileNameOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.FileName, new FileInfo(defaultValue))
    {
    }

    protected override string ValueToString()
    {
      return Value.FullName;
    }
  }
}