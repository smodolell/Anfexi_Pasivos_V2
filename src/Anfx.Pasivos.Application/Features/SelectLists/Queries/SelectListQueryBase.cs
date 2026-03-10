namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public abstract class SelectListQueryBase : IQuery<Result<List<SelectItemDto>>>
{
    public string? SearchTerm { get; set; }
    public int? MaxResults { get; set; }
}

