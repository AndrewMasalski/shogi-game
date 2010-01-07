using System.Globalization;
using System.Windows.Controls;

namespace Yasc.Gui
{
  public class StringRangeValidationRule : ValidationRule
  {
    public StringRangeValidationRule()
    {
      MinimumLength = -1;
      MaximumLength = -1;
    }

    public int MinimumLength { get; set; }
    public int MaximumLength { get; set; }
    public string ErrorMessage { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      var result = new ValidationResult(true, null);
      string inputString = (value ?? string.Empty).ToString();
      if (inputString.Length < MinimumLength || (MaximumLength > 0 && inputString.Length > MaximumLength))
      {
        result = new ValidationResult(false, ErrorMessage);
      }
      return result;
    }
  }
}