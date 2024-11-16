using System;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

// This class is used to represent a paginated list of items of any type (using the generic type 'T').
public class PageList<T> : List<T> // 'T' is used to make this class generic (i.e., it can work with any type of data).
{
    // Constructor to initialize the paginated list with items, total count, page number, and page size.
    public PageList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        // Set the current page number.
        CurrentPage = pageNumber;

        // Calculate the total number of pages needed based on the total count of items and page size.
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        // Set the page size (number of items per page).
        PageSize = pageSize;

        // Set the total count of items (not just the current page's items).
        TotalCount = count;

        // Add the provided items (i.e., the current page's items) to the list.
        AddRange(items);
    }

    // Property to store the current page number (e.g., page 1, 2, etc.).
    public int CurrentPage { get; set; }

    // Property to store the total number of pages available.
    public int TotalPages { get; set; }

    // Property to store the number of items per page.
    public int PageSize { get; set; }

    // Property to store the total number of items in the dataset.
    public int TotalCount { get; set; }

    // Static method to create a paginated list asynchronously from a data source (e.g., database).
    public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        // Get the total count of items in the data source.
        var count = await source.CountAsync();

        // Fetch only the items needed for the current page.
        // 'Skip' moves the cursor past the previous pages' items.
        // 'Take' grabs the number of items for the current page.
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        // Create and return a new instance of 'PageList<T>' with the fetched items and metadata.
        return new PageList<T>(items, count, pageNumber, pageSize);
    }
}