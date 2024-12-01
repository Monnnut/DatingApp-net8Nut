using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);

    Task<Message?> GetMessage(int id);

    Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);


    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection?> GetConnection(string connectionId);

    Task<Group?> GetMessageGroup(string groupName);

    Task<Group?> GetGroupForConnection(string connectionId);
}
