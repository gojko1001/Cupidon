using DatingApp.Utils.Pagination;
using System.Text.Json;

namespace DatingApp.Extensions;

public static class HttpExtensions
{
    private readonly static JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
        var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, serializerOptions));
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
