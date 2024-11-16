using System;

namespace API.Helpers;

// This class is used to hold parameters for pagination when fetching data from an API or database.
public class UserParams : PaginationParams
{
   

    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; } // exclude user themselves

    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 70;
    public string OrderBy { get; set; } = "lastActive";
}

// This class is typically used in APIs to handle pagination parameters from client requests.
// By using the UserParams class:
// You can control how many items are returned per page.
// You prevent clients from requesting too many items at once, which helps optimize performance and server resources.