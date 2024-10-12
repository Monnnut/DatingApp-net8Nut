using System;


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
}


