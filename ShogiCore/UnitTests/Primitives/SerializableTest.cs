using CommonUtils.UnitTests;
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
      Assert.AreSame(PT.王, PT.王.SerializeDeserialize());
      Assert.AreSame(PT.玉, PT.玉.SerializeDeserialize());
      Assert.AreSame(PT.飛, PT.飛.SerializeDeserialize());
      Assert.AreSame(PT.角, PT.角.SerializeDeserialize());
      Assert.AreSame(PT.金, PT.金.SerializeDeserialize());
      Assert.AreSame(PT.銀, PT.銀.SerializeDeserialize());
      Assert.AreSame(PT.桂, PT.桂.SerializeDeserialize());
      Assert.AreSame(PT.香, PT.香.SerializeDeserialize());
      Assert.AreSame(PT.歩, PT.歩.SerializeDeserialize());
      Assert.AreSame(PT.竜, PT.竜.SerializeDeserialize());
      Assert.AreSame(PT.馬, PT.馬.SerializeDeserialize());
      Assert.AreSame(PT.全, PT.全.SerializeDeserialize());
      Assert.AreSame(PT.今, PT.今.SerializeDeserialize());
      Assert.AreSame(PT.仝, PT.仝.SerializeDeserialize());
      Assert.AreSame(PT.と, PT.と.SerializeDeserialize());
    }
  }
}