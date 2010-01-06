namespace DotUsi
{
  public abstract class UsiOptionBase
  {
    public UsiEngine Engine { get; private set; }
    public string Name { get; private set; }
    public UsiOptionType OptionType { get; private set; }

    internal abstract string CommitCommand { get; }

    protected UsiOptionBase(UsiEngine engine, string name, UsiOptionType optionType)
    {
      Engine = engine;
      Name = name;
      OptionType = optionType;
    }
  }
}