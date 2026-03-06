namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Menu
{
    public PSV_Menu()
    {
        this.Rol = new HashSet<Rol>();
    }

    public int ID { get; set; }
    public string Titulo { get; set; }
    public string Area { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Icon { get; set; }
    public decimal Orden { get; set; }
    public Nullable<int> ParentID { get; set; }
    public bool Activo { get; set; }

    public virtual ICollection<Rol> Rol { get; set; }
}
