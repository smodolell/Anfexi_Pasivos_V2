namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class AnticipoDto
{
    public int IdTipoReduccion { get; set; }
    public int IdContrato { get; set; }
    public DateTime FechaAnticipo { get; set; }
    public decimal MontoAnticipo { get; set; }
    public int IdTipoTerminacion { get; set; }
    public decimal MontoInteres { get; set; }
    public decimal MontoIVA_Interes { get; set; }
    public decimal MontoPena { get; set; }
    public decimal MontoIVA_Pena { get; set; }
    public decimal MontoTotal { get; set; }
    public bool EsLiquidacion { get; set; }

}
