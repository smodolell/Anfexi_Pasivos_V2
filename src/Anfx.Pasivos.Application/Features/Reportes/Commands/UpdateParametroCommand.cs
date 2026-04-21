using Anfx.Pasivos.Application.Common;
using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Commands;

public class UpdateParametroCommand : ICommand<Result>
{
    public required ParametroEditDto Model { get; set; }
}

public class UpdateParametroCommandHandler(
    IApplicationDbContext context
) : ICommandHandler<UpdateParametroCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(UpdateParametroCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = message.Model;
            var oParametro = await _context.RSP_Parametro
                .SingleOrDefaultAsync(r => r.Id == dto.ParametroId, cancellationToken);

            if (oParametro == null)
                return Result.NotFound("El parámetro no existe.");

            oParametro.Display = dto.Display;
            oParametro.InputId = dto.InputId ?? 0;
            oParametro.Order = dto.Order;

            if (ApplicationConstants.RSP_Input_DropDownList == dto.InputId)
            {
                oParametro.TablaRef = dto.TablaRef ?? "";
                oParametro.ColumnaValor = dto.ColumnaValor ?? "";
                oParametro.ColumnaTexto = dto.ColumnaTexto ?? "";
            }
            else
            {
                oParametro.TablaRef = "";
                oParametro.ColumnaValor = "";
                oParametro.ColumnaTexto = "";
            }

            _context.RSP_Parametro.Update(oParametro);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("Los datos se almacenaron correctamente.");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
