using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Concurrent;

namespace redis_clone;

public class Server
{
    private TcpListener server = new TcpListener(IPAddress.Any, 6379);
    private ConcurrentDictionary<string, (string value, DateTime? dateTime)> redisCache = new();

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

            HandleCommand(socket, arguments);
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

    private void HandleCommand(Socket socket, List<string> arguments)
    {
        var command = arguments[0].ToUpper();

        switch (command)
        {
            case "PING":
                socket.Send(Encoding.ASCII.GetBytes("+PONG\r\n"));
                break;
            case "ECHO":
                var resp = arguments[1];
                socket.Send(Encoding.ASCII.GetBytes($"+{resp}\r\n"));
                break;
            case "SET":
                SetValue(socket, arguments);
                break;
            case "GET":
                GetValue(socket, arguments);
                break;
            default:
                socket.Send(Encoding.ASCII.GetBytes("+UNKNOWN\r\n"));
                break;
        }
    }

    private void GetValue(Socket socket, List<string> arguments)
    {
        var key = arguments[1];
        if (redisCache.ContainsKey(key))
        {
            var value = redisCache[key];

            if (value.dateTime.HasValue && DateTime.Now >= value.dateTime.Value)
            {
                redisCache.TryRemove(key, out _);
                socket.Send(Encoding.ASCII.GetBytes("$-1\r\n"));
                return;
            }

            socket.Send(Encoding.ASCII.GetBytes($"+{value.value}\r\n"));
        }
        else
        {
            socket.Send(Encoding.ASCII.GetBytes("$-1\r\n"));
        }
    }

    private void SetValue(Socket socket, List<string> arguments)
    {
        var key = arguments[1];
        var value = arguments[2];

        if (arguments.Count == 5 && arguments[3].ToUpper() == "PX")
        {
            int expiry = int.Parse(arguments[4]);
            redisCache.TryAdd(key, (value, DateTime.Now.AddMilliseconds(expiry)));
        }

        redisCache.TryAdd(key, (value, null));

        socket.Send(Encoding.ASCII.GetBytes("+OK\r\n"));
    }
}
