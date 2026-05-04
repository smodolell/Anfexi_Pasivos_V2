namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Bitacora
{
    public int ID { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Pantalla { get; set; }= string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public System.DateTime FechaOperacion { get; set; }
}
