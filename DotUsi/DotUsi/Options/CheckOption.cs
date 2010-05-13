using Yasc.DotUsi.Options.Base;

namespace Yasc.DotUsi.Options
{
  /// <summary>Represents option of type <see cref="UsiOptionType.Check"/></summary>
  public class CheckOption : ValueOptionBase<bool>
  {
    /// <summary>Ctor engine uses when parse engine response</summary>
    internal CheckOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.Check, bool.Parse(defaultValue))
    {
    }
    /// <summary>Ctor drivers use to create "implicit" options</summary>
    internal CheckOption(UsiEngine engine, string name, bool alwaysPass)
      : base(engine, name, UsiOptionType.Check, false)
    {
      IsImplicit = true;
      AlwaysPass = alwaysPass;
    }
    /// <summary>Override to specify how the <see cref="ValueOptionBase{T}.Value"/> should be represented in USI command</summary>
    protected override string ValueToString()
    {
      return Value.ToString().ToLower();
    }
  }
}