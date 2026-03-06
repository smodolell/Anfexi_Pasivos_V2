namespace Anfx.Pasivos.Application.Features.Roles.Specifications;

public class RolSearchSpec : Specification<Rol>
{

    public RolSearchSpec(string? searchTerm,bool activo = true)
    {

        if (activo) 
        {
            Query.Where(r => r.Activo);
        }
        else
        {
            Query.Where(r => !r.Activo);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            Query.Where(r => r.sRol.Contains(searchTerm) || r.Descripcion.Contains(searchTerm));
        }

    }


}
