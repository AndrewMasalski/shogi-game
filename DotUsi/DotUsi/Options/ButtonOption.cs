namespace DotUsi
{
  public class ButtonOption : UsiOptionBase
  {
    public ButtonOption(UsiEngine engine, string name)
      : base(engine, name, UsiOptionType.Button)
    {
    }

    public void Press()
    {
      Engine.SetOption(this, false);
    }


    public override string ToString()
    {
      return Name + "()";
    }

    internal override string CommitCommand
    {
      get { return "setoption " + Name; }
    }
  }
}