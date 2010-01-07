namespace DotUsi
{
  ///<summary>Base class for all USI options</summary>
  public abstract class UsiOptionBase
  {
    ///<summary>Owner engine reference</summary>
    public UsiEngine Engine { get; private set; }
    ///<summary>Option name</summary>
    public string Name { get; private set; }
    ///<summary>Option type</summary>
    public UsiOptionType OptionType { get; private set; }

    internal abstract string CommitCommand { get; }

    /// <summary>ctor</summary>
    protected UsiOptionBase(UsiEngine engine, string name, UsiOptionType optionType)
    {
      Engine = engine;
      Name = name;
      OptionType = optionType;
    }
  }
}