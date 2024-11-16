using System;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;


namespace API.Helpers;
// This class implements an asynchronous action filter to log user activity.
// It uses the IAsyncActionFilter interface, which allows you to perform custom logic 
// before or after an action method executes in an ASP.NET Core application.
public class LogUserActivity : IAsyncActionFilter
{
    // The method that gets called before and/or after an action method executes.
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Proceed with the execution of the action method (i.e., let the request continue).
        var resultContext = await next();

        // Check if the user is authenticated (i.e., logged in).
        // If the user is not authenticated, exit the method and do nothing.
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        // Get the username of the currently authenticated user using an extension method.
        var userid = resultContext.HttpContext.User.GetUserId();

        // Get an instance of IUserRepository from the dependency injection container.
        var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

        // Fetch the user object from the database using the username.
        var user = await repo.GetUserByIdAsync(userid);

        // If the user is not found (which shouldn't normally happen), exit the method.
        if (user == null) return;

        // Update the user's "LastActive" property with the current UTC time.
        user.LastActive = DateTime.UtcNow;

        // Save the changes to the database.
        await repo.SaveAllAsync();
    }
}
