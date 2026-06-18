using System.Net;
using System.Net.Sockets;

namespace Aco228.Common.Helpers;

public static class LocalIpFinder
{
    public static string GetLocalIPAddress()
    {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint.Address.ToString();
        }
    }
}