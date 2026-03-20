using Anfx.Pasivos.Application.Features.Auth.DTOs;
using Mapster;

namespace Anfx.Pasivos.Application.Features.Auth;

public class AuthMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UsuarioCreateDto, Usuario>()
            .Map(o => o.NombreCompleto, d => d.NombreCompleto)
            .Map(o => o.Email, d => d.Email)
            .Map(o => o.UserName, d => d.UsuarioNombre)
            .Map(o => o.IdRol, d => d.RolId);

        config.NewConfig<Usuario, UsuarioDto>()
            .Map(o => o.Id, d => d.IdUsuario)
            .Map(o => o.UsuarioNombre, d => d.UserName)
            .Map(o => o.FechaRegistro, d => d.FechaRegistracion)
            .Map(o => o.RolId, d => d.IdRol)
            .Map(o => o.Rol.Id, d => d.Rol.IdRol)
            .Map(o => o.Rol.Descripcion, d => d.Rol.Descripcion)
            .Map(o => o.Rol.sRol, d => d.Rol.Titulo)
            ;


        config.NewConfig<Rol, RolDto>()
          .Map(o => o.Id, d => d.IdRol)
          .Map(o => o.sRol, d => d.Titulo)
           ;
        config.NewConfig<RolDto, Rol>()
            .Map(o => o.Titulo, d => d.sRol)
            .Map(o => o.Descripcion, d => d.Descripcion);
    }
}

  