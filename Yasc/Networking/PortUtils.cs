using System;
using System.Net;
using System.Net.Sockets;

namespace Yasc.Networking
{
  public static class PortUtils
  {
    static PortUtils()
    {
      LocalHost = new IPAddress(new byte[] { 127, 0, 0, 1 });
    }

    private static readonly IPAddress LocalHost;
    private const int AddressInUseError = 10048;

    public static bool IsPortInUse(int port)
    {
      var s = new Socket(AddressFamily.InterNetwork,
                         SocketType.Stream, ProtocolType.Tcp);
      try
      {
        s.Bind(new IPEndPoint(LocalHost, port));
        s.Close();
        return false;
      }
      catch (SocketException ex)
      {
        if (ex.ErrorCode == AddressInUseError)
          return true;

        throw;
      }
    }
    public static int GetFreePortToListen()
    {
      for (int i = 1024; i < 65536; i++)
        if (!IsPortInUse(i))
          return i;
      throw new Exception("All ports are used");
    }
  }
}