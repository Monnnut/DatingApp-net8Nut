using System;
using API.DTOs;
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

    Task<bool> SavedAllAsync();
}
