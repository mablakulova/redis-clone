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

        var socket = server.AcceptSocket();
        byte[] response = Encoding.ASCII.GetBytes("+PONG\r\n");
        socket.Send(response);

        Console.WriteLine("Logs from your program will appear here!");
    }
}
