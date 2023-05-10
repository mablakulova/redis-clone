# Redis in C#

A multi-threaded Redis clone that supports basic commands like SET, GET, PING, and ECHO.

## How to use

1. Redis client is not supported on Windows and will need WSL2 to be installed on the Windows environment.
2. Ensure you have dotnet (6.0) installed locally.
3. Go to where `redis-clone.csproj` file located and run the server in the command line: ``` dotnet run --project . --configuration Release "$@" ```
4. Input commands into the client command line (see below)

## Features
See below the features included in this implementation of Redis.

### Ping
Ping the server to check if it is responsive.
```
PING
```

### Echo
Get a response from the server.
```
ECHO <value>
```

### Get
Get the value corresponding to the key in the data store.
```
GET <key>
```

### Set
Insert into the data store a key-value pair with an optional expiration time. PX notes milliseconds.
```
SET <key> <value> [PX <time>]
```
