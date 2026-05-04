namespace Anfx.Pasivos.Domain.Entities;

public class PSV_FondeadoresCuenta
{
    public int Id { get; set; }

    public int? IdFondeador { get; set; }

    public string Cuenta { get; set; } = string.Empty;

    public virtual PSV_Fondeador? IdFondeadorNavigation { get; set; }

    public virtual ICollection<PSV_ContratoMinistracione> PSV_ContratoMinistraciones { get; set; } = new List<PSV_ContratoMinistracione>();
}
