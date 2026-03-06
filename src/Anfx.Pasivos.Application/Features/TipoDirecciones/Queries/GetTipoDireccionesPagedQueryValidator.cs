namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;

public class GetTipoDireccionesPagedQueryValidator : AbstractValidator<GetTipoDireccionesPagedQuery>
{
    public GetTipoDireccionesPagedQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La página debe ser mayor o igual a 1");

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El tamaño debe ser mayor o igual a 1")
            .LessThanOrEqualTo(50)
            .WithMessage("El tamaño máximo permitido es 50 registros por página");
    }
}