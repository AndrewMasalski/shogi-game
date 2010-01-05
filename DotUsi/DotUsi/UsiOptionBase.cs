using System;
using System.Collections.ObjectModel;
using System.IO;

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

  public class ButtonOption : UsiOptionBase
  {
    public ButtonOption(UsiEngine engine, string name)
      : base(engine, name, UsiOptionType.Button)
    {
    }

    public void Press()
    {
      Engine.SetOption(this, false);
    }


    public override string ToString()
    {
      return Name + "()";
    }

    internal override string CommitCommand
    {
      get { return "setoption " + Name; }
    }
  }

  public class SpinOption : ValueOptionBase<int>
  {
    public SpinOption(UsiEngine engine, string name, string defaultValue, string min, string max)
      : base(engine, name, UsiOptionType.Spin, int.Parse(defaultValue))
    {
      Min = min == null ? (int?)null : int.Parse(min);
      Max = max == null ? (int?)null : int.Parse(max);
    }
    public override int Value
    {
      get { return base.Value; }
      set
      {
        if (Max != null || value > Max) 
          throw new ArgumentOutOfRangeException("value");
        if (Min != null || value < Min) 
          throw new ArgumentOutOfRangeException("value");

        base.Value = value;
      }
    }
    public int? Min { get; private set; }
    public int? Max { get; private set; }
  }

  public class ComboOption : ValueOptionBase<string>
  {
    public ReadOnlyCollection<string> PossibleValues { get; private set; }

    public ComboOption(UsiEngine engine, string name, string defaultValue, ReadOnlyCollection<string> possibleValues)
      : base(engine, name, UsiOptionType.Combo, defaultValue)
    {
      PossibleValues = possibleValues;
    }

    public override string Value
    {
      get { return base.Value; }
      set
      {
        if (!PossibleValues.Contains(value))
          throw new ArgumentOutOfRangeException("value");

        base.Value = value;
      }
    }
  }

  public class StringOption : ValueOptionBase<string>
  {
    public StringOption(UsiEngine engine, string name, string defaultValue)
      : base(engine, name, UsiOptionType.String, defaultValue)
    {
    }
  }

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