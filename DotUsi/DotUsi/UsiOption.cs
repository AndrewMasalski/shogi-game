using System.Collections.ObjectModel;

namespace DotUsi
{
  public class UsiOption
  {
    public string Name { get; private set; }
    public UsiOptionType OptionType { get; private set; }
    public string CurrentValue { get; private set; }
    public string Min { get; private set; }
    public string Max { get; private set; }
    public ReadOnlyCollection<string> PossibleValues { get; private set; }

    public UsiOption(string name, UsiOptionType optionType, string defaultValue, string min, string max, ReadOnlyCollection<string> possibleValues)
    {
      Name = name;
      OptionType = optionType;
      CurrentValue = defaultValue;
      Min = min;
      Max = max;
      PossibleValues = possibleValues;
    }
    internal void UpdateCurrentValue(string value)
    {
      CurrentValue = value;
    }
    public override string ToString()
    {
      return Name + " = " + CurrentValue;
    }
  }
}