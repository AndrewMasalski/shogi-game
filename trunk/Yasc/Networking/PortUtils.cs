using System;
using System.Net;
using System.Net.Sockets;

namespace Yasc.Networking
{
  public static class PortUtils
  {
    private static readonly IPAddress _localHost
      = new IPAddress(new byte[] { 127, 0, 0, 1 });

    private const int PosrtIsBusy = 10048;

    public static bool IsPortBusy(int port)
    {
      var s = new Socket(AddressFamily.InterNetwork,
                         SocketType.Stream, ProtocolType.Tcp);
      try
      {
        s.Bind(new IPEndPoint(_localHost, port));
        s.Close();
        return false;
      }
      catch (SocketException ex)
      {
        if (ex.ErrorCode == PosrtIsBusy)
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