using System;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        //step 1 sort the message and make it querable
        var query = context.Messages
        .OrderByDescending(x => x.MessageSent)
        .AsQueryable();

        //query message depending on which container they click on
        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.UserName && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.UserName && x.SenderDeleted == false),
            _ => query.Where(x => x.Recipient.UserName == messageParams.UserName && x.DateRead == null && x.RecipientDeleted == false)
        };
        //convert message into Message Dto with mapper
        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

        //createasync with pagination 
        return await PageList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        //get the message
        var messages = await context.Messages
        //make query to join sender, then photos
        .Include(x => x.Sender).ThenInclude(x => x.Photos) //optimaize later
        //make queru to join recipient, the photo
        .Include(x => x.Recipient).ThenInclude(x => x.Photos)
        //get the query location
        .Where(x =>
              x.RecipientUsername == currentUsername 
              && x.RecipientDeleted == false 
              && x.SenderUsername == recipientUsername ||
              x.SenderUsername == currentUsername 
              && x.SenderDeleted == false 
              && x.RecipientUsername == recipientUsername)
        //arrange the message
        .OrderBy(x => x.MessageSent)
        //saved to async to database
        .ToListAsync();


        //find unread messages and create a list
        var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        //map to dto
        return mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SavedAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
