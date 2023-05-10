using System.Net.Sockets;
using System.Net;

namespace redis_clone;

public class Server
{
    private TcpListener server = new TcpListener(IPAddress.Any, 6379);

    public void Start()
    {
        server.Start();
        server.AcceptSocket(); // wait for client

        Console.WriteLine("Logs from your program will appear here!");
    }
}
