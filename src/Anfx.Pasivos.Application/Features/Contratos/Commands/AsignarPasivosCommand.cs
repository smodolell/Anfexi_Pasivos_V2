namespace Anfx.Pasivos.Application.Features.Contratos.Commands;

public class AsignarPasivosCommand : ICommand<Result>
{
    public int IdContratoPasivo { get; set; }
    public int[]? ListaContratos { get; set; }
    public int? ContratosActivos { get; set; }
}
internal class AsignarPasivosCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork) : ICommandHandler<AsignarPasivosCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(AsignarPasivosCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var itemDb = await _context.PSV_Contrato
                .Include(i => i.PSV_RelActivoPasivo)
                .FirstOrDefaultAsync(r => r.IdContrato == message.IdContratoPasivo);

            if (itemDb == null) return Result.NotFound("No se ha encontrado el contrato pasivo a relacionar.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            if (message.ListaContratos == null && message.ContratosActivos != null)
            {
                message.ListaContratos = new int[] { message.ContratosActivos.Value };
            }

            itemDb.PSV_RelActivoPasivo.Clear();

            if (message.ListaContratos != null && message.ListaContratos.Any())
            {
                foreach (var item in message.ListaContratos)
                {
                    var asociado = await _context.PSV_RelActivoPasivo.FirstOrDefaultAsync(f => f.IdContratoActivo == item 
                    && f.IdContratoPasivo != message.IdContratoPasivo);

                    if (asociado == null)
                    {
                        itemDb.PSV_RelActivoPasivo.Add(new PSV_RelActivoPasivo { 
                            IdContratoPasivo = itemDb.IdContrato, 
                            IdContratoActivo = item, 
                            IdUsuario_Asigno = 1,//Luego implementar 
                            FechaAsignacion = DateTime.Now
                        });
                    }
                }
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.SuccessWithMessage("Datos Almacenados Correctamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Error(ex.Message);
        }
    }
}
