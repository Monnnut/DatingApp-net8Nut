using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessageRepository messageRepository, IUserRepository userRepository,
IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if (Context.User == null || string.IsNullOrEmpty(otherUser)) throw new Exception("Cannot join group");

        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);

        await Clients.Caller.SendAsync("RecieveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        //step 1 get username first of the on accessing
        var username = Context.User?.GetUsername() ?? throw new Exception("could not get user");
        //step 2 check whether they are trying to send message to themselves
        if (username == createMessageDto.RecipientUsername)
        {
            throw new HubException("You cannot message yourself");
        }

        //step 3 establish who is send and receiver
        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        //step 4 check whether they exist
        if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
        {
            throw new HubException("Cannot send messege");
        }

        //create Message object with data to be send to Dbcontext
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messageRepository.GetMessageGroup(groupName);

        if(group != null && group.Connections.Any(x =>x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        } else
        {
            var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
            if(connections != null && connections?.Count != null)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecieved",new 
                {username= sender.UserName, knownAs = sender.KnownAs});
            }
        }

        //send to databse and check whether if saved async before mapping
        messageRepository.AddMessage(message);
        if (await messageRepository.SavedAllAsync())
        {
            
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }

    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
        var group = await messageRepository.GetMessageGroup(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

        if (group == null)
        {
            group = new Group { Name = groupName };
            messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

      if (await messageRepository.SavedAllAsync()) return group;

      throw new HubException("Fail to join group");
    }


    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(X=> X.ConnectionId == Context.ConnectionId);

        if (connection != null && group !=null)
        {
            messageRepository.RemoveConnection(connection);
            if (await messageRepository.SavedAllAsync()) return group;
        }
        throw new Exception("Fail to remove from group");
    }

    private string GetGroupName(string caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }



}