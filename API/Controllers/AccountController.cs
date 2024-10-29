using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // account/register=> endpoint
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) //using normal string paramter will not work as http does not know where to get the paramter. Thus we need DTOs (DATA TRANSFER OBJECT)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("UserName is taken");
        //using hashing algorithmn to incript some text
        //using the dispose method will be called once the class is not user
       
        return Ok();

        // using var hmac = new HMACSHA512();

        // //HMAC (Hash-based Message Authentication Code)

        // var user = new AppUser
        // {

        //     UserName = registerDto.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        //     //above syntax convert password ib UTF-8 encoding since
        //     // computers process data as bytes, it is necessary to convert
        //     //password into a format the system can use to create Hash
        //     //afte that, hmac.ComputeHash takes those bytes and run them
        //     //through hashing algorithmn SHA512 to product Hash output
        //     PasswordSalt = hmac.Key // to sort our password

        // };
        // //put data in database
        // context.Users.Add(user);
        // //save database
        // await context.SaveChangesAsync();

        // return new UserDto
        // {
        //     Username = user.UserName,
        //     Token = tokenService.CreateToken(user)
        // };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
        if (user == null) return Unauthorized("Invalid username");

        // user passwordSalt to get the same Hash as database
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");

        }
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }
    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
