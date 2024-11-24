using System;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppRoles : IdentityRole<int>
//create relationship between roles and users
//create joint table between approles and appusers
{
    public ICollection<AppUserRoles> UserRoles { get; set; } = [];
}
