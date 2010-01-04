namespace DotUsi
{
  public class SfenString
  {
    public string Value { get; private set; }

    /// <summary>
    /// TODO: Constructor must validate value it's provided with!
    /// </summary>
    /// <param name="value"></param>
    public SfenString(string value)
    {
      Value = value;
    }
    public static implicit operator string(SfenString s)
    {
      return s.Value;
    }
    public static implicit operator SfenString(string s)
    {
      return new SfenString(s);
    }
  }
}