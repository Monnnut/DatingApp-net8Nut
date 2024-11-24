using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        }).ToListAsync();

        return Ok(users);
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        //make sure user has roles
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");

        //selected roles
        var selectedRoles = roles.Split(",").ToArray(); //The Split(",") method splits the roles string into an array of strings at each comma.
        //find the selected user by passing username as parameter
        var user = await userManager.FindByNameAsync(username);
        //check if user exist
        if (user == null) return BadRequest("User not found");
        //find user current roles
        var userRoles = await userManager.GetRolesAsync(user);
        //add the selected role except if it is already assign
        var results = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        //check to see if it succedd
        if (!results.Succeeded) return BadRequest("Fail to add to role");
        //if succeed remove the previous roles except for the newly selected role
        results = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        //check if the role is successfully removed
        if (!results.Succeeded) return BadRequest("Fail to remove roles");
        //return the update role to client
        return Ok(await userManager.GetRolesAsync(user));

    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or Moderators can see this");
    }

}
