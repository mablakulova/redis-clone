﻿namespace redis_clone;

public class Program
{
    static void Main(string[] args)
    {
        var server = new Server();
        server.Start();
    }
}
