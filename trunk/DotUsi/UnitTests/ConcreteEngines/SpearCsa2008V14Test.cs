using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.DotUsi;
using Yasc.DotUsi.Drivers;
using Yasc.DotUsi.Options;
using Yasc.DotUsi.Process;

namespace DotUsi.UnitTests.ConcreteEngines
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