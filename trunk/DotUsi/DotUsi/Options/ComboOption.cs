using System;
using System.Collections.ObjectModel;
using Yasc.DotUsi.Options.Base;

namespace Yasc.DotUsi.Options
{
  /// <summary>Represents option of type <see cref="UsiOptionType.Combo"/></summary>
  public class ComboOption : ValueOptionBase<string>
  {
    /// <summary>Combo drop-down list values</summary>
    public ReadOnlyCollection<string> PossibleValues { get; private set; }

    internal ComboOption(UsiEngine engine, string name, string defaultValue, ReadOnlyCollection<string> possibleValues)
      : base(engine, name, UsiOptionType.Combo, defaultValue)
    {
      PossibleValues = possibleValues;
    }

    /// <summary>Current option value</summary>
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