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
            var command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var arguments = DecodeCommand(command);

            if (arguments[0].ToUpper() == "PING")
            {
                socket.Send(Encoding.ASCII.GetBytes("+PONG\r\n"));
            }
            else if (arguments[0].ToUpper() == "ECHO")
            {
                var resp = arguments[1];
                socket.Send(Encoding.ASCII.GetBytes($"+{resp}\r\n"));
            }
            else
            {
                socket.Send(Encoding.UTF8.GetBytes("+UNKNOWN\r\n"));
            }
        }
    }

    private List<string> DecodeCommand(string command)
    {
        if (command[0] != '*')
        {
            throw new ArgumentException("Invalid command format.");
        }

        var lines = command.Split(new[] { "\r\n" }, StringSplitOptions.None);
        var numberOfArguments = int.Parse(lines[0].Substring(1));
        var arguments = new List<string>();

        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("$"))
            {
                arguments.Add(lines[i + 1]);
                i++;
            }
        }

        if (arguments.Count != numberOfArguments)
        {
            throw new ArgumentException("Invalid command format.");
        }

        return arguments;
    }
}
