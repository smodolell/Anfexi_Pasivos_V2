namespace Anfx.Pasivos.Application.Common.DTOs;

public class PagedResultDto<T>
      where T : class, new()
{
    public IEnumerable<T> Results { get; set; } = Enumerable.Empty<T>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}