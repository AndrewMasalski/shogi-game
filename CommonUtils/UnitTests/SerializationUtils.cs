using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommonUtils.UnitTests
{
  public static class SerializationUtils
  {
    public static T SerializeDeserialize<T>(this T root)
    {
      var f = new BinaryFormatter();
      var s = new MemoryStream();
      f.Serialize(s, root);
      s.Position = 0;
      return (T)f.Deserialize(s);
    }
  }
}