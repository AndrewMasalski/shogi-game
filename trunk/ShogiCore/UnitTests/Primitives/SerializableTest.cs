using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Primitives
{
  [TestClass]
  public class SerializableTest
  {
    [TestMethod]
    public void CheckSingletone()
    {
      Assert.AreSame(PT.王, SerializrDeserialize(PT.王));
      Assert.AreSame(PT.玉, SerializrDeserialize(PT.玉));
      Assert.AreSame(PT.飛, SerializrDeserialize(PT.飛));
      Assert.AreSame(PT.角, SerializrDeserialize(PT.角));
      Assert.AreSame(PT.金, SerializrDeserialize(PT.金));
      Assert.AreSame(PT.銀, SerializrDeserialize(PT.銀));
      Assert.AreSame(PT.桂, SerializrDeserialize(PT.桂));
      Assert.AreSame(PT.香, SerializrDeserialize(PT.香));
      Assert.AreSame(PT.歩, SerializrDeserialize(PT.歩));
      Assert.AreSame(PT.竜, SerializrDeserialize(PT.竜));
      Assert.AreSame(PT.馬, SerializrDeserialize(PT.馬));
      Assert.AreSame(PT.全, SerializrDeserialize(PT.全));
      Assert.AreSame(PT.今, SerializrDeserialize(PT.今));
      Assert.AreSame(PT.仝, SerializrDeserialize(PT.仝));
      Assert.AreSame(PT.と, SerializrDeserialize(PT.と));
    }

    private static T SerializrDeserialize<T>(T root)
    {
      var f = new BinaryFormatter();
      var s = new MemoryStream();
      f.Serialize(s, root);
      s.Position = 0;
      return (T) f.Deserialize(s);
    }
  }
}