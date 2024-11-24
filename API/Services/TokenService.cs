using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access tokenkey from appsetting");  //two question mark to check nullness
        if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));  // same key use to encript and decript

        if (user.UserName == null)
        {
            throw new Exception("No Username for user");
        }

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.UserName)
    };

        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        //The signing credentials specify how the token will be secured.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        //A SecurityTokenDescriptor is used to define how the token will be structured:
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Subject contains the claims (user information).
            Subject = new ClaimsIdentity(claims),
            //Expires sets the token's expiration date to 7 days from the current time.
            Expires = DateTime.UtcNow.AddDays(7),
            //SigningCredentials is the key and algorithm used to sign the token.
            SigningCredentials = creds
        };

        //This would generate the token and return it as a string.
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
