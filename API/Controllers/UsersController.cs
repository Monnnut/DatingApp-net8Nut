using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

//[Authorize]
public class UsersController(DataContext context) : BaseApiController
{

    //ActionResult VS IActionResult
    //interface that represents results of an action
    //ActiocResuls is a generic type that allows you to return result
    [AllowAnonymous] // override authenticating
    [HttpGet]
    //IEnumerable is a generic interface represents a collection
    //of objects that you can iterate over. List/Arrays
    //no modification of the list
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        //get users information from DBset<Users> and list it
        var users = await context.Users.ToListAsync();

        //ActionResult allow us to retuen a Http Reponses
        return Ok(users);
    }
    [Authorize]
    [HttpGet("{id}")] // /api/users/{id}

    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}
