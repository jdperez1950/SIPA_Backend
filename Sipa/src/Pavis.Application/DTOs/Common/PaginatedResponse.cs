namespace Pavis.Application.DTOs.Common;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public PaginationMeta Meta { get; set; } = new();
}

public class PaginationMeta
{
    public int TotalItems { get; set; }
    public int ItemCount { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
