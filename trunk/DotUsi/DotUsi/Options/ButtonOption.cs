using System;

namespace DotUsi
{
  /// <summary>Represents option of type <see cref="UsiOptionType.Button"/></summary>
  public class ButtonOption : UsiOptionBase
  {
    internal ButtonOption(UsiEngine engine, string name)
      : base(engine, name, UsiOptionType.Button)
    {
    }
    /// <summary>Call to send engine signal that the button is pressed</summary>
    public void Press()
    {
      Engine.SetOption(this, false);
    }

    /// <summary> Gets the user friendly representation of the option</summary>
    public override string ToString()
    {
      return Name + "()";
    }

    internal override string CommitCommand
    {
      get { return "setoption name " + Name; }
    }

    /// <summary>If <see cref="UsiOptionBase.IsImplicit"/> sends default 
    ///   value of the option to the engine, otherwice raises exception!</summary>
    /// <exception cref="InvalidOperationException">IsImplicit</exception>
    public override void SetImplicitValue()
    {
      if (!IsImplicit) throw new InvalidOperationException("IsImplicit != true");
      Press();
    }
  }
}