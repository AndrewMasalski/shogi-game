using System;
using System.Collections.ObjectModel;

namespace DotUsi
{
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
}