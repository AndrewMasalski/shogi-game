using System.Windows.Markup;

namespace Chess
{
  /// <summary>
  /// PositionValueSerializer - ValueSerializer class for converting instances of strings to 
  /// and from Position instances. This is used by the MarkupWriter class. 
  /// </summary>
  public class PositionValueSerializer : ValueSerializer
  {
    /// <summary>
    /// Returns true. 
    /// </summary>
    public override bool CanConvertFromString(string value, IValueSerializerContext context)
    {
      return true;
    }

    /// <summary> 
    /// Returns true if the given value can be converted into a string
    /// </summary> 
    public override bool CanConvertToString(object value, IValueSerializerContext context)
    {
      // Validate the input type 
      return value is Position;
    }

    /// <summary>
    /// Converts a string into a Position. 
    /// </summary> 
    public override object ConvertFromString(string value, IValueSerializerContext context)
    {
      
      return Position.Parse(value);
      
    }

    /// <summary>
    /// Converts the value into a string. 
    /// </summary>
    public override string ConvertToString(object value, IValueSerializerContext context)
    {
      if (value is Position)
      {
        var instance = (Position)value;
        return instance.ToString();
      }

      return base.ConvertToString(value, context);
    }
  }
}