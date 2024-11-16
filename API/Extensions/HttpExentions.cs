using System;
using System.Text.Json;
using API.Helpers;

namespace API.Extensions;

// This static class contains extension methods for HTTP responses.
public static class HttpExtentions
{
    // Extension method to add pagination information to the HTTP response headers.
    // The 'this HttpResponse' part means you can call this method directly on an HttpResponse object.
    public static void AddPaginationHeader<T>(this HttpResponse response, PageList<T> data)
    {
        // Create a PaginationHeader object containing pagination details (current page, page size, total items, total pages).
        var paginationHeader = new PaginationHeader(
            data.CurrentPage,  // The current page number
            data.PageSize,     // The number of items per page
            data.TotalCount,   // The total number of items
            data.TotalPages    // The total number of pages
        );

        // Define JSON serialization options to convert the header to camelCase format (common in JSON APIs).
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // Serialize the pagination header to JSON and add it to the response headers under the key "Pagination".
        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));

        // Allow the "Pagination" header to be exposed to the client (e.g., in browsers) for cross-origin requests.
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
