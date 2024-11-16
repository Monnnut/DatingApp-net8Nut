using System;
using API.Extensions;


namespace API.Entities;

public class AppUser
{
    //access modifiler (Public, Private, Internal)
    //get value and set value
    //each property represent column in a table

    // int is primitive type
    // use [Key] when primary key is not name Id
    //entity framework use auto increment number when new data is added
    public int Id { get; set; }

    //string is reference type
    public required string UserName { get; set; }
    public byte[] PasswordHash { get; set; } = [];

    public byte[] PasswordSalt { get; set; } = [];

    public DateOnly DateOfBirth { get; set; }

    public required string KnownAs { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public required string Gender { get; set; }

    public string? Interests { get; set; }

    public string? LookingFor { get; set; }

    public required string City { get; set; }
    public required string Country { get; set; }

    public List<Photo> Photos { get; set; } = []; //one to many relationship/ navigation properties

    public List<UserLike> LikedByUsers { get; set; } = [];

     public List<UserLike> LikeUsers { get; set; } = [];
}
