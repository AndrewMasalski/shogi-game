using System.Diagnostics;
using System.Linq;
using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  [TestClass]
  public class SpearCsa2009V15Test
  {
    [TestMethod]
    public void UsiEngineConstructorTest()
    {
      foreach (var p in Process.GetProcessesByName("SpearShogidokoro"))
        p.Kill();

      IUsiProcess process = new UsiWindowsProcess(Utils.Relative(@"Spear\SpearShogidokoro.exe"));
      process = new SpearCsa2009V15Driver(process);
      using (var engine = new UsiEngine(process))
      {
        engine.DebugMode = true;
        engine.SynchUsi();
        engine.Options.Where(o => o.Name == "SpearLevel").Cast<SpinOption>().First().Value = 1;
        engine.SynchIsReady();
        engine.SynchNewGame();
        engine.Position(
          "7g7f", "3c3d", "7f7e", "8b3b", "2h7h", "5a6b", "5i4h", "6b7b", "4h3h", "7b8b", "3h2h",
          "7a7b", "8h2b+", "3a2b", "3i3h", "3d3e", "7i6h", "3e3f", "7e7d", "7c7d", "7h7d", "3f3g+",
          "3h3g", "P*3f", "3g4h", "B*5e", "B*4f", "5e4f", "4g4f", "B*6e", "P*3c", "2b3c", "7d7g",
          "3f3g+", "2i3g", "P*3f", "B*3h", "3f3g+", "4h3g", "6e3h+", "2h3h", "B*6e", "B*5f", "6e5f",
          "5g5f", "3c4d", "P*3c", "4d3c", "7g7d", "3c4d", "P*3c", "4d3c", "4f4e", "3c4b", "3h4h",
          "P*7c", "7d7f", "N*2e", "3g4f", "B*3g", "4f3g", "3b3g+", "4h5i", "S*3h", "4i4h", "3h4g+",
          "4h3g", "2e3g+", "6i5h", "G*4h", "5h4h", "3g4h", "5i6i", "G*8h", "G*5g", "4g5g", "B*7a",
          "8b7a", "B*6b", "7a6b", "7f7c+", "7b7c"
          );
        engine.SynchGo();
      }
    }
  }
}