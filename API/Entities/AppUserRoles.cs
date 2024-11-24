using System;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUserRoles : IdentityUserRole<int>
{
    public AppUser User { get; set; } = null!;
    public AppRoles Role { get; set; } = null!;
}
