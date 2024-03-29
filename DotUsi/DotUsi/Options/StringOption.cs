using Yasc.DotUsi.Options.Base;

namespace Yasc.DotUsi.Options
{
  /// <summary>Represents option of type <see cref="UsiOptionType.String"/></summary>
  public class StringOption : ValueOptionBase<string>
  {
    internal StringOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.String, defaultValue)
    {
    }
  }
}