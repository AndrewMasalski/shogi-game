namespace Yasc.DotUsi
{
  /// <summary>Represents SFEN string as described in standard</summary>
  public class SfenString
  {
    /// <summary>SFEN string</summary>
    public string Value { get; private set; }

    /// <summary>
    /// TODO: Constructor must validate value it's provided with!
    /// </summary>
    /// <param name="value"></param>
    public SfenString(string value)
    {
      Value = value;
    }
    /// <summary>Gets SFEN string</summary>
    public static implicit operator string(SfenString s)
    {
      return s.Value;
    }
    /// <summary>Gets SFEN string</summary>
    public static implicit operator SfenString(string s)
    {
      return new SfenString(s);
    }
  }
}