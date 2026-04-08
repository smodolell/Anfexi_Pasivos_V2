namespace Anfx.Pasivos.Application.Features.SelectLists.Specifications;

public class TipoMovimientoSpec : Specification<TipoMovimiento>
{
    public TipoMovimientoSpec(bool? capturable)
    {
        if (capturable.HasValue)
        {
            Query.Where(w => w.Capturable == capturable.Value);
        }
    }
}
