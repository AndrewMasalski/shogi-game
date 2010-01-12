using System.Collections.Generic;

namespace Yasc.Persistence
{
  public class GameTrascription
  {
    public Dictionary<string, TrascriptionProperty> Properties { get; private set; }

    public bool IsFull
    {
      get { return Properties.Count > 0 && Moves.Count > 0; }
    }

    public List<string> Moves { get; private set; }

    public GameTrascription()
    {
      Properties = new Dictionary<string, TrascriptionProperty>();
      Moves = new List<string>();
    }

    public void AddProperty(TrascriptionProperty property)
    {
      Properties[property.Name] = property;
    }
  }
}