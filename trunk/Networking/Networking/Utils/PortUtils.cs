using System;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels.Tcp;

namespace Yasc.Networking.Utils
{
  public static class PortUtils
  {
    public static bool IsPortBusy(int port)
    {
      try
      {
        new TcpChannel(port).StopListening(null);
        return false;
      }
      catch (SocketException ex)
      {
        if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
          return true;

        throw;
      }
    }
    public static int GetFreePortToListen()
    {
      for (int i = 1024; i < 65536; i++)
        if (!IsPortBusy(i))
          return i;
      throw new Exception("All ports are busy");
    }
  }
}