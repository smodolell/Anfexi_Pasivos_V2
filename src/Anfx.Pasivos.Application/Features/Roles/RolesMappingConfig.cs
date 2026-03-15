using Anfx.Pasivos.Application.Features.Roles.DTOs;
using Mapster;

namespace Anfx.Pasivos.Application.Features.Roles;



public class RolesMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Rol, RolDto>()
            .Map(o => o.Id, d => d.IdRol)
            .Map(o => o.sRol, d => d.Titulo)
             ;
        config.NewConfig<RolDto, Rol>()
            .Map(o => o.Titulo, d => d.sRol)
            .Map(o => o.Descripcion, d => d.Descripcion);
    }
}
