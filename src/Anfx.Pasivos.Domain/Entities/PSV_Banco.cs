namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Banco
{

    public int IdBanco { get; set; }
    public string Banco { get; set; } = string.Empty;

    public virtual ICollection<PSV_CuentaBancaria> PSV_CuentaBancaria { get; set; } = new List<PSV_CuentaBancaria>();
}
