namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Archivo
{
    public System.Guid ID { get; set; }
    public string NombreArchivo { get; set; }
    public byte[] Contenido { get; set; }
    public bool Activo { get; set; }
}
