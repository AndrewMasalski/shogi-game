using System;

namespace DotUsi
{
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
}