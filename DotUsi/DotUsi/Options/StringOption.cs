namespace DotUsi
{
  public class StringOption : ValueOptionBase<string>
  {
    public StringOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.String, defaultValue)
    {
    }
  }
}