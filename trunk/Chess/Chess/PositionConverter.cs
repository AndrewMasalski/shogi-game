using System;
using System.ComponentModel;
using System.Globalization;

namespace Chess
{
  /// <summary> PositionConverter - Converter class for converting instances of other
  ///   types to and from <see cref="Position"/> instances </summary> 
  public sealed class PositionConverter : TypeConverter
  {
    /// <summary> Returns true if this type converter can convert from a given type.</summary> 
    /// <returns>
    /// bool - True if this converter can convert from the provided type, false if not.
    /// </returns>
    /// <param name="context"> The ITypeDescriptorContext for this call. </param> 
    /// <param name="sourceType"> The Type being queried for support. </param>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string))
      {
        return true;
      }

      return base.CanConvertFrom(context, sourceType);
    }

    /// <summary> 
    /// Returns true if this type converter can convert to the given type.
    /// </summary> 
    /// <returns>
    /// bool - True if this converter can convert to the provided type, false if not.
    /// </returns>
    /// <param name="context"> The ITypeDescriptorContext for this call. </param> 
    /// <param name="destinationType"> The Type being queried for support. </param>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        return true;
      }

      return base.CanConvertTo(context, destinationType);
    }

    /// <summary> 
    /// Attempts to convert to a Position from the given object.
    /// </summary> 
    /// <returns>
    /// The Position which was constructed.
    /// </returns>
    /// <exception cref="NotSupportedException"> 
    /// A NotSupportedException is thrown if the example object is null or is not a valid type
    /// which can be converted to a Position. 
    /// </exception> 
    /// <param name="context"> The ITypeDescriptorContext for this call. </param>
    /// <param name="culture"> The requested CultureInfo.  Note that conversion uses "en-US" rather than this parameter. </param> 
    /// <param name="value"> The object to convert to an instance of Position. </param>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null)
      {
        throw GetConvertFromException(value);
      }

      var source = value as string;

      if (source != null)
      {
        return Position.Parse(source);
      }

      return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// ConvertTo - Attempt to convert an instance of Position to the given type
    /// </summary>
    /// <returns> 
    /// The object which was constructoed.
    /// </returns> 
    /// <exception cref="NotSupportedException"> 
    /// A NotSupportedException is thrown if "value" is null or not an instance of Position,
    /// or if the destinationType isn't one of the valid destination types. 
    /// </exception>
    /// <param name="context"> The ITypeDescriptorContext for this call. </param>
    /// <param name="culture"> The CultureInfo which is respected when converting. </param>
    /// <param name="value"> The object to convert to an instance of "destinationType". </param> 
    /// <param name="destinationType"> The type to which this will convert the Position instance. </param>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (value is Position)
      {
        var instance = (Position)value;

        if (destinationType == typeof(string))
        {
          // Delegate to the formatting/culture-aware ConvertToString method.
          return instance.ToString();
        }
      }

      // Pass unhandled cases to base class (which will throw exceptions for null value or destinationType.)
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}