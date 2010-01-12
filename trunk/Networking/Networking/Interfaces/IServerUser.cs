namespace Yasc.Networking
{
  public interface IServerUser
  {
    string Name { get; }
    IServerGame CurrentGame { get; }
  }
}