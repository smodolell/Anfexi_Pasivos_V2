using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetAutocompleteContratoQuery : IQuery<Result<List<AutocompleteResultDto>>>
{
    public string? Search { get; set; }
}

internal class GetAutocompleteContratoQueryHandler (IApplicationDbContext context): IQueryHandler<GetAutocompleteContratoQuery, Result<List<AutocompleteResultDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<AutocompleteResultDto>>> HandleAsync(GetAutocompleteContratoQuery message, CancellationToken cancellationToken = default)
    {
        var result = new List<AutocompleteResultDto>();
        if (!string.IsNullOrEmpty(message.Search))
        {
            var dbResult = _context.View_PSV_PasivoAutocomple
                .Where(w => w.label.Contains(message.Search))
                .Take(15);

            result = await dbResult.Select(s=>new AutocompleteResultDto
            {
                label = s.label,
                value = s.value,
            }).ToListAsync();

        }
        return Result.Success(result);
    }
}
