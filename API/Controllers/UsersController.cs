using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]// /api/users
public class UsersController(DataContext context) : ControllerBase
{

    //ActionResult VS IActionResult
    //interface that represents results of an action
    //ActiocResuls is a generic type that allows you to return result
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
