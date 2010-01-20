using System;

namespace DotUsi
{
  /// <summary>Represents option of type <see cref="UsiOptionType.Spin"/></summary>
  public class SpinOption : ValueOptionBase<int>
  {
    internal SpinOption(UsiEngine engine, string name, string defaultValue, string min, string max)
      : base(engine, name, UsiOptionType.Spin, int.Parse(defaultValue))
    {
      Min = min == null ? (int?)null : int.Parse(min);
      Max = max == null ? (int?)null : int.Parse(max);
    }
    /// <summary>Ctor drivers use to create "implicit" options</summary>
    internal SpinOption(UsiEngine engine, string name, bool alwaysPass, int defaultValue)
      : base(engine, name, UsiOptionType.Check, defaultValue)
    {
      IsImplicit = true;
      AlwaysPass = alwaysPass;
    }
    /// <summary>Current option value</summary>
    public override int Value
    {
      get { return base.Value; }
      set
      {
        if (Max != null && value > Max) 
          throw new ArgumentOutOfRangeException("value");
        if (Min != null && value < Min) 
          throw new ArgumentOutOfRangeException("value");

        base.Value = value;
      }
    }
    /// <summary>Spin value lower bound</summary>
    public int? Min { get; private set; }
    /// <summary>Spin value upper bound</summary>
    public int? Max { get; private set; }
  }
}