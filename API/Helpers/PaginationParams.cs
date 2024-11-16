using System;

namespace API.Helpers;

public class PaginationParams
{
 // A constant that defines the maximum number of items allowed per page.
    private const int MaxPageSize = 50;

    // Property to store the current page number. By default, it starts at page 1.
    public int PageNumber { get; set; } = 1;

    // A private field to store the page size (i.e., how many items per page).
    private int _pageSize = 10;

    // Public property to get or set the page size.
    // It uses a "getter" and "setter" to control the value.
    public int PageSize
    {
        get => _pageSize; // Getter: returns the current page size.

        // Setter: If the value set by the user is greater than the maximum allowed,
        // it will set the page size to the maximum (MaxPageSize). Otherwise, it uses the provided value.
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
