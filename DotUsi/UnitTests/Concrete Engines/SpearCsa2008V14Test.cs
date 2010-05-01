using System.Diagnostics;
using System.Linq;
using DotUsi;
using DotUsi.Drivers;
using DotUsi.Options;
using DotUsi.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  [TestClass]
  public class SpearCsa2008V14Test
  {
    [TestMethod]
    public void UsiEngineConstructorTest()
    {
      foreach (var p in Process.GetProcessesByName("SpearShogidokoro"))
        p.Kill();

      IUsiProcess process = new UsiWindowsProcess(@"Spear\SpearShogidokoro.exe");
      process = new SpearCsa2008V14Driver(process);
      using (var engine = new UsiEngine(process))
      {
        engine.DebugMode = true;
        engine.SynchUsi();
        engine.SetImplicitOptions();
        engine.Options.Where(o => o.Name == "SpearLevel").Cast<SpinOption>().First().Value = 1;
        engine.SynchIsReady();
        engine.SynchNewGame();
        engine.Position("7g7f");
        engine.SynchGo();
      }
    }
  }
}