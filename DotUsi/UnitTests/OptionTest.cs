using System;
using System.Collections.Generic;
using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  [TestClass]
  public class ButtonOptionTest : OptionTest<ButtonOption>
  {
    [TestMethod]
    public void TestButtonOption()
    {
      Option.Press();
      Assert.AreEqual("setoption SomeName", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type button";
    }
  }
  
  [TestClass]
  public class CheckOptionTest : OptionTest<CheckOption>
  {
    [TestMethod]
    public void TestCheckOption()
    {
      Assert.IsTrue(Option.Value);
      
      Option.Value = false;
      Assert.AreEqual("setoption SomeName value false", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Option.Value = false;
      Assert.AreEqual(0, InputData.Count);
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.IsFalse(Option.Value);

      Option.Value = true;
      Assert.AreEqual("setoption SomeName value true", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.IsTrue(Option.Value);
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type check default true";
    }
  }
  
  [TestClass]
  public class SimplestSpinOptionTest : OptionTest<SpinOption>
  {
    [TestMethod]
    public void TestSimplestSpinOption()
    {
      Assert.AreEqual(300, Option.Value);
      
      Option.Value = 200;
      Assert.AreEqual("setoption SomeName value 200", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Option.Value = 200;
      Assert.AreEqual(0, InputData.Count);
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual(200, Option.Value);

      Option.Value = 350;
      Assert.AreEqual("setoption SomeName value 350", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual(350, Option.Value);
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type spin default 300";
    }
  }
  
  [TestClass]
  public class RangeSpinOptionTest : OptionTest<SpinOption>
  {
    [TestMethod]
    public void TestRangeSpinOption()
    {
      Assert.AreEqual(-200, Option.Min);
      Assert.AreEqual(300, Option.Max);
    } 

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestMinSpinOption()
    {
      Option.Value = 400;
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestMaxSpinOption()
    {
      Option.Value = -400;
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type spin default 300 min -200 max 300";
    }
  }
  
  [TestClass]
  public abstract class OptionTest<T>
    where T : UsiOptionBase
  {
    [TestInitialize]
    public virtual void Init()
    {
      Process = new TestProcess();
      Engine = new UsiEngine(Process);

      Engine.Usi();
      Process.SendOutput(GetOptionDefinition());
      Process.SendOutput("usiok");
      Process.InputData.Clear();
      
      Assert.AreEqual(1, Engine.Options.Count);
      Assert.IsTrue(Engine.Options[0] is T);
    }

    protected UsiEngine Engine { get; private set; }
    protected TestProcess Process { get; private set; }
    protected Queue<string> InputData { get { return Process.InputData; } }
    protected T Option { get { return (T)Engine.Options[0]; } }

    protected abstract string GetOptionDefinition();
  }
}