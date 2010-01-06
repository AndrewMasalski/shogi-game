namespace DotUsi
{
  public abstract class ValueOptionBase<T> : UsiOptionBase
  {
    private T _value;

    protected ValueOptionBase(UsiEngine engine, string name, UsiOptionType optionType, T defaultValue)
      : base(engine, name, optionType)
    {
      _value = defaultValue;
    }

    /// <remarks>Not case sensitive, cannot contain spaces</remarks>
    public virtual T Value
    {
      get { return _value; }
      set
      {
        if (Equals(_value, value)) return;
        _value = value;
        Engine.SetOption(this, true);
      }
    }

    internal override string CommitCommand
    {
      get { return "setoption " + Name + " value " + ValueToString(); }
    }

    public override string ToString()
    {
      return Name + " = " + Value;
    }
    protected virtual string ValueToString()
    {
      return Value.ToString();
    }
  }
}