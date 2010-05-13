using System;
using System.Collections.Generic;
using DotUsi.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.DotUsi;
using Yasc.DotUsi.Options;
using Yasc.DotUsi.Options.Base;

namespace DotUsi.UnitTests
{
  [TestClass]
  public class ButtonOptionTest : OptionTestBase<ButtonOption>
  {
    [TestMethod]
    public void TestButtonOption()
    {
      Assert.AreEqual(UsiOptionType.Button, Option.OptionType);
      Option.Press();
      Assert.AreEqual("setoption name SomeName", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type button";
    }
  }
  
  [TestClass]
  public class CheckOptionTest : OptionTestBase<CheckOption>
  {
    [TestMethod]
    public void TestCheckOption()
    {
      Assert.AreEqual(UsiOptionType.Check, Option.OptionType);
      Assert.IsTrue(Option.Value);
      
      Option.Value = false;
      Assert.AreEqual("setoption name SomeName value false", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Option.Value = false;
      Assert.AreEqual(0, InputData.Count);
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.IsFalse(Option.Value);

      Option.Value = true;
      Assert.AreEqual("setoption name SomeName value true", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.IsTrue(Option.Value);
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type check default true";
    }
  }
  
  [TestClass]
  public class SimplestSpinOptionTest : OptionTestBase<SpinOption>
  {
    [TestMethod]
    public void TestSimplestSpinOption()
    {
      Assert.AreEqual(UsiOptionType.Spin, Option.OptionType);
      Assert.AreEqual(300, Option.Value);
      
      Option.Value = 200;
      Assert.AreEqual("setoption name SomeName value 200", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Option.Value = 200;
      Assert.AreEqual(0, InputData.Count);
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual(200, Option.Value);

      Option.Value = 350;
      Assert.AreEqual("setoption name SomeName value 350", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual(350, Option.Value);
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type spin default 300";
    }
  }
  
  [TestClass]
  public class ComboOptionTest : OptionTestBase<ComboOption>
  {
    [TestMethod]
    public void TestComboOption()
    {
      Assert.AreEqual(UsiOptionType.Combo, Option.OptionType);
      Assert.AreEqual("300", Option.Value);
      CollectionAssert.AreEqual(new []{"100", "200", "300", "400"}, Option.PossibleValues);
      
      Option.Value = "200";
      Assert.AreEqual("setoption name SomeName value 200", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Option.Value = "200";
      Assert.AreEqual(0, InputData.Count);
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual("200", Option.Value);

      Option.Value = "400";
      Assert.AreEqual("setoption name SomeName value 400", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual("400", Option.Value);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestInvalidOption()
    {
      Option.Value = "201";
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type combo default 300 var 100 var 200 var 300 var 400";
    }
  }


  [TestClass]
  public class StringOptionTest : OptionTestBase<StringOption>
  {
    [TestMethod]
    public void TestStringOption()
    {
      Assert.AreEqual(UsiOptionType.String, Option.OptionType);
      Assert.AreEqual("300", Option.Value);

      Option.Value = "200";
      Assert.AreEqual("setoption name SomeName value 200", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Option.Value = "200";
      Assert.AreEqual(0, InputData.Count);
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual("200", Option.Value);

      Option.Value = "400";
      Assert.AreEqual("setoption name SomeName value 400", InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, Engine.Mode);

      Assert.AreEqual("400", Option.Value);
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type string default 300";
    }
  }
  [TestClass]
  public class RangeSpinOptionTest : OptionTestBase<SpinOption>
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
    [TestMethod]
    public void TestValidValue()
    {
      Option.Value = 0;
    }

    protected override string GetOptionDefinition()
    {
      return "option name SomeName type spin default 300 min -200 max 300";
    }
  }
  
  [TestClass]
  public abstract class OptionTestBase<T>
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