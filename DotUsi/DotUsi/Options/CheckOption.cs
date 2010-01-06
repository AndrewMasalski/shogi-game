namespace DotUsi
{
  public class CheckOption : ValueOptionBase<bool>
  {
    public CheckOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.Check, bool.Parse(defaultValue))
    {
    }
    protected override string ValueToString()
    {
      return Value.ToString().ToLower();
    }
  }
}