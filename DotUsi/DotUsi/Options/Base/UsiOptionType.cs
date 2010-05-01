namespace DotUsi.Options.Base
{
  /// <summary>Type of an option USI engine can have</summary>
  public enum UsiOptionType
  {
    /// <summary>A checkbox that can either be true or false.</summary>
    Check,
    /// <summary>A spin wheel or slider that can be an integer in a certain range.</summary>
    Spin,
    /// <summary>A combo box that can have different predefined strings as a value.</summary>
    Combo,
    /// <summary>A button that can be pressed to send a command to the engine</summary>
    Button,
    /// <summary>A text field that has a string as a value, an empty string has the value "&lt;empty&gt;".</summary>
    String,
    /// <summary>Similar to <see cref="String"/>, but is presented as a file browser instead of a text field in the GUI.</summary>
    FileName
  }
}