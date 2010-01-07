using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  /// <summary>
  /// TODO: Verifies that synch and asynch versions of operations are really synch and asynch
  /// </summary>
  [TestClass]
  public class UsiEngineAsynchOperationsTest
  {
    private TestProcess _process;
    private UsiEngine _engine;

    [TestInitialize]
    public void Init()
    {
      _process = new TestProcess();
      _engine = new UsiEngine(_process);
    }
  }
}