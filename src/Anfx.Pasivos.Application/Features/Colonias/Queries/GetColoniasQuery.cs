using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public record GetColoniasQuery : IQuery<Result<IEnumerable<ColoniaDto>>>;
