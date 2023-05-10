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

        while (true)
        {
            var socket = server.AcceptSocket();
            ThreadPool.QueueUserWorkItem(HandleClient, socket);
        }
    }

    private void HandleClient(object? socketObject)
    {
        var socket = (Socket)socketObject;
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = socket.Receive(buffer)) > 0)
        {
            byte[] response = Encoding.ASCII.GetBytes("+PONG\r\n");
            socket.Send(response);
        }
    }
}
