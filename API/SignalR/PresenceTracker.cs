using System;
using System.Security.Cryptography.X509Certificates;

namespace API.SignalR;

public class PresenceTracker
{
    //Dictionary takes key and value pair
    //store connected users
    private static readonly Dictionary<string, List<string>> OnlineUsers = [];

    public Task<bool> UserConnected(string username, string connectionId)
    {
        var isOnline = false;
        //nothing else can update this
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, [connectionId]);
                isOnline = true;
            }
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string username, string connectionId)
    {
        var isOffline = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username))
            {
                return Task.FromResult(isOffline);
            }

            OnlineUsers[username].Remove(connectionId);

            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    public static Task<List<string>> GetConnectionForUser(string username)
    {
        List<string> connectionid;
        if (OnlineUsers.TryGetValue(username, out var connections))
        {
            lock (connections)
            {
                connectionid = [.. connections];
            }
        }
        else
        {
            connectionid = [];
        }

        return Task.FromResult(connectionid);
    }

}
