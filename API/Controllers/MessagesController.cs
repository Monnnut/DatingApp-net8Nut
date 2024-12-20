using System;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Authorize]
public class MessagesController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController

{
    [HttpPost]//create message
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        //step 1 get username first of the on accessing
        var username = User.GetUsername();
        //step 2 check whether they are trying to send message to themselves
        if (username == createMessageDto.RecipientUsername)
        {
            return BadRequest("You cannot message yourself");
        }

        //step 3 establish who is send and receiver
        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        //step 4 check whether they exist
        if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
        {
            return BadRequest("Cannot send messege");
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

        //send to databse and check whether if saved async before mapping
        unitOfWork.MessageRepository.AddMessage(message);
        if (await unitOfWork.Complete()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        //Step 1 get username
        messageParams.UserName = User.GetUsername();

        //step 2 get the message. Data logic in repo
        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);


        //Step 3 get messages and pagination detail to the client with HTML headers
        Response.AddPaginationHeader(messages);

        //step 4 return the message 
        return messages;


    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await unitOfWork.MessageRepository.GetMessage(id);

        if (message == null) return BadRequest("Cannot delete this message");

        if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
        {
            unitOfWork.MessageRepository.DeleteMessage(message);
        }

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the message");


    }

}
