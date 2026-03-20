namespace Anfx.Pasivos.Application.Features.Auth.Specifications;

public class RolSearchSpec : Specification<Rol>
{

    public RolSearchSpec(string? searchTerm, bool activo = true)
    {

        //if (activo)
        //{
        //    Query.Where(r => r.Activo);
        //}
        //else
        //{
        //    Query.Where(r => !r.Activo);
        //}

        if (!string.IsNullOrEmpty(searchTerm))
        {
            Query.Where(r => (r.Titulo != null && r.Titulo.Contains(searchTerm)) || (r.Descripcion != null && r.Descripcion.Contains(searchTerm)));
        }

    }


}
