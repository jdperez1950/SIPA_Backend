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

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
}
