using System.Net.Sockets;
using System.Net;
using System.Text;

namespace redis_clone;

public class Server
{
    private TcpListener server = new TcpListener(IPAddress.Any, 6379);

    public void Start()
    {
        server.Start();
        Console.WriteLine("Logs will appear here!");

        var socket = server.AcceptSocket();
        HandleClient(socket);
    }

    private static void HandleClient(Socket socket)
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = socket.Receive(buffer)) > 0)
        {
            byte[] response = Encoding.ASCII.GetBytes("+PONG\r\n");
            socket.Send(response);
        }
    }
}
