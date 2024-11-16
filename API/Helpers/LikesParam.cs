using System;

namespace API.Helpers;

public class LikesParam : PaginationParams
{
    public int UserId { get; set; }
    public required string Predicate { get; set; } = "liked";
}
