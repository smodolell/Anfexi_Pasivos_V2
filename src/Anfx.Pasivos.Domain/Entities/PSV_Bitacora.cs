namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Bitacora
{
    public int ID { get; set; }
    public string Usuario { get; set; }
    public string Pantalla { get; set; }
    public string Accion { get; set; }
    public string Descripcion { get; set; }
    public System.DateTime FechaOperacion { get; set; }
}
