namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Archivo
{
    public Guid ID { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public byte[] Contenido { get; set; } = Array.Empty<byte>();
    public bool Activo { get; set; }
}
