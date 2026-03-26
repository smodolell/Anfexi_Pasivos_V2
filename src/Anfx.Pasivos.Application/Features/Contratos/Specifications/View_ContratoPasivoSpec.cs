namespace Anfx.Pasivos.Application.Features.Contratos.Specifications;

public class View_ContratoPasivoSpec : Specification<View_ContratoPasivo>
{

    public View_ContratoPasivoSpec(int? idFondeador,int? idEstatusContrato,int? idLineaCredito,string? searchText)
    {
        if (idFondeador.HasValue)
        {
            Query.Where(r => r.FondeadorID == idFondeador.Value);
        }

        if (idEstatusContrato.HasValue)
        {
            Query.Where(r => r.IdEstatusContrato == idEstatusContrato.Value);
        }

        if (idLineaCredito.HasValue)
        {
            Query.Where(r => r.IdLineaCredito == idLineaCredito.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            Query.Where(r =>
                r.Fondeador.Contains(searchText) ||
                r.Contrato.Contains(searchText));
        }
    }
}
