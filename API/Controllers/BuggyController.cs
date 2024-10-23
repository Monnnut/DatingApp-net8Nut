using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "secret text";
    }


    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var things = context.Users.Find(-1);
        if (things == null) return NotFound(); //404 not found
        return things;
    }


    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError()
    {
        var thing = context.Users.Find(-1) ?? throw new Exception("A bad thing has happened");
        return thing;
    }

    [HttpGet("bad-request")]
    public ActionResult<AppUser> GetBadRequest() // error 400-499 range are considered user side error
    {
        return BadRequest("this was not a good request");
    }

}
