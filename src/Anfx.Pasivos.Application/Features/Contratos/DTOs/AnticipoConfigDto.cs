namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class AnticipoConfigDto
{

    public bool EsLiquidacion { get; set; }
    public decimal MontoAnticipo { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal PorcIVA_Interes { get; set; }
    public decimal PorcIVA_Pena { get; set; }
    public bool CalculaInteres { get; set; }
    public bool PermitePena { get; set; }


}
