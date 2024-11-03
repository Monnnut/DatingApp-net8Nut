using System;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
{

    //ActionResult VS IActionResult
    //interface that represents results of an action
    //ActiocResuls is a generic type that allows you to return result
    // [AllowAnonymous] // override authenticating
    [HttpGet]
    //IEnumerable is a generic interface represents a collection
    //of objects that you can iterate over. List/Arrays
    //no modification of the list
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        //get users information from DBset<Users> and list it
        var users = await userRepository.GetMemberAsync();

        //ActionResult allow us to retuen a Http Reponses
        return Ok(users);
    }

    [HttpGet("{username}")] // /api/users/{id}

    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username);
        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (username == null)
        {
            return BadRequest("No username found in token");
        }

        var user = await userRepository.GetUserByUsernameAsync(username);

        if(user == null){
            return BadRequest("Could not find user.");
        }

        mapper.Map(memberUpdateDto, user);

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }
}
