namespace Anfx.Pasivos.Domain.Entities;

public class View_TipoTablaAmortiza
{
    public string TipoTablaAmortiza { get; set; } = string.Empty;
    public int ID { get; set; }
    public bool EsCapitalizable { get; set; }
    public bool Activo { get; set; }
}
