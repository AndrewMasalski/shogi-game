namespace DotUsi
{
  /// <summary>Represents option of type <see cref="UsiOptionType.Check"/></summary>
  public class CheckOption : ValueOptionBase<bool>
  {
    /// <summary>ctor</summary>
    internal CheckOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.Check, bool.Parse(defaultValue))
    {
    }
    /// <summary>Override to specify how the <see cref="ValueOptionBase{T}.Value"/> should be represented in USI command</summary>
    protected override string ValueToString()
    {
      return Value.ToString().ToLower();
    }
  }
}