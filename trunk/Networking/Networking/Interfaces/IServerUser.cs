namespace Yasc.Networking.Interfaces
{
  public interface IServerUser
  {
    string Name { get; }
    IServerGame CurrentGame { get; }
  }
}