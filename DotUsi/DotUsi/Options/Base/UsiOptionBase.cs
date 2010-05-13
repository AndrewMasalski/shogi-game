using System;

namespace Yasc.DotUsi.Options.Base
{
  ///<summary>Base class for all USI options</summary>
  public abstract class UsiOptionBase
  {
    ///<summary>Owner engine reference</summary>
    public UsiEngine Engine { get; private set; }
    ///<summary>Option name</summary>
    public string Name { get; private set; }
    ///<summary>Option type</summary>
    public UsiOptionType OptionType { get; private set; }
    ///<summary>
    ///   True, if driver created that option, 
    ///   false if engine reported about it
    /// </summary>
    public bool IsImplicit { get; protected set; }
    ///<summary>
    ///   True, if engine won't work well if you don't pass that option 
    ///   (default value is invalid than)
    ///   false if engine can leave without it
    /// </summary>
    public bool AlwaysPass { get; protected set; }

    internal abstract string CommitCommand { get; }

    /// <summary>ctor</summary>
    protected UsiOptionBase(UsiEngine engine, string name, UsiOptionType optionType)
    {
      Engine = engine;
      Name = name;
      OptionType = optionType;
    }

    /// <summary>If <see cref="IsImplicit"/> sends default 
    ///   value of the option to the engine, otherwice raises exception!</summary>
    /// <exception cref="InvalidOperationException">IsImplicit</exception>
    public abstract void SetImplicitValue();
  }
}