using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context) //Http context represent all incoming HTTP request(like headers, URL, user info )
   //async Task means methd run asynchronously and return a promise to complete
   //its work later without blocking other tasks
    {
        try
        {
            await next(context); // waits for the next middleware to finish processing the request
        }
        catch (Exception ex)
        {

            logger.LogError(ex, ex.Message); // record the error ex to keep track
            context.Response.ContentType = "application/json";//set response type to JSON, send readable format to api
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //This sets the HTTP status code of the response to 500, which means "Internal Server Error."

            var response = env.IsDevelopment()// check whether if its in dev mode
            ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)
            : new ApiExceptions(context.Response.StatusCode, ex.Message, "Internal server error");
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);

        }
    }
}

//RequestDelegate points a method to handle a HTTP request
//log information like errors or messages during the application's execution.
//IHostEnvironment allows you to check if the app is running in development, production, or another environment.
// In simple terms: this code sets up a special class to handle errors
// in a web app. It takes in tools to manage the next action 
// (the next delegate), log error messages (the logger), and check the 
// environment (whether it's running in development mode or production).