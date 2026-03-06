namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Banco
{

    public int IdBanco { get; set; }
    public string Banco { get; set; }

    public virtual ICollection<PSV_CuentaBancaria> PSV_CuentaBancaria { get; set; }
}
