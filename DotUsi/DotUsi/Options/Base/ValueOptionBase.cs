using System;

namespace Yasc.DotUsi.Options.Base
{
  ///<summary>Base class for options with scalar value</summary>
  ///<typeparam name="T">value type</typeparam>
  public abstract class ValueOptionBase<T> : UsiOptionBase
  {
    private T _value;

    /// <summary>ctor</summary>
    protected ValueOptionBase(UsiEngine engine, string name, UsiOptionType optionType, T defaultValue)
      : base(engine, name, optionType)
    {
      _value = defaultValue;
    }

    /// <summary>Current option value</summary>
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
      get { return "setoption name " + Name + " value " + ValueToString(); }
    }

    /// <summary>Get's user friendly representation of option</summary>
    public override string ToString()
    {
      return Name + " = " + Value;
    }
    /// <summary>Override to specify how the <see cref="Value"/> should be represented in USI command</summary>
    protected virtual string ValueToString()
    {
      return Value.ToString();
    }

    /// <summary>If <see cref="UsiOptionBase.IsImplicit"/> sends default 
    ///   value of the option to the engine, otherwice raises exception!</summary>
    /// <exception cref="InvalidOperationException">IsImplicit</exception>
    public override void SetImplicitValue()
    {
      if (!IsImplicit) throw new InvalidOperationException("IsImplicit != true");
      Engine.SetOption(this, true);
    }
  }
}