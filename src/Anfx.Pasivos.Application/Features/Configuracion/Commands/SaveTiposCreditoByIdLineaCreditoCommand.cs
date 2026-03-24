using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class SaveTiposCreditoByIdLineaCreditoCommand : ICommand<Result>
{
    public int IdLineaCredito { get; set; }
    public required List<RelLineaCreditoTipoCreditoDto> Model { get; set; }
}
public class SaveTiposCreditoByIdLineaCreditoCommandValidator : AbstractValidator<SaveTiposCreditoByIdLineaCreditoCommand>
{
    private readonly IApplicationDbContext _context;

    public SaveTiposCreditoByIdLineaCreditoCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.IdLineaCredito)
            .GreaterThan(0).WithMessage("El ID de la línea de crédito debe ser mayor a 0")
            .MustAsync(LineaCreditoExists).WithMessage("La línea de crédito especificada no existe");

        RuleFor(x => x.Model)
            .NotNull().WithMessage("La lista de tipos de crédito es requerida")
            .NotEmpty().WithMessage("Debe especificar al menos un tipo de crédito");

        // Validar que no hay IDs duplicados en la lista
        RuleFor(x => x.Model)
            .Must(NoDuplicateIds)
            .WithMessage("No pueden existir tipos de crédito duplicados en la lista");

       
        // Validación adicional: verificar que todos los tipos de crédito existen en una sola consulta
        RuleFor(x => x.Model)
            .MustAsync(AllTiposCreditoExist)
            .WithMessage("Uno o más tipos de crédito no existen en el sistema");
    }

    private async Task<bool> LineaCreditoExists(int idLineaCredito, CancellationToken cancellationToken)
    {
        return await _context.PSV_LineaCredito
            .AnyAsync(x => x.IdLineaCredito == idLineaCredito, cancellationToken);
    }

    private bool NoDuplicateIds(List<RelLineaCreditoTipoCreditoDto> model)
    {
        if (model == null || !model.Any())
            return true;

        var duplicateIds = model
            .GroupBy(x => x.IdTipoCredito)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        return !duplicateIds.Any();
    }

    private async Task<bool> AllTiposCreditoExist(List<RelLineaCreditoTipoCreditoDto> model, CancellationToken cancellationToken)
    {
        if (model == null || !model.Any())
            return true;

        var ids = model.Select(x => x.IdTipoCredito).Distinct().ToList();
        var existingIds = await _context.TipoCredito
            .Where(x => ids.Contains(x.IdTipoCredito))
            .Select(x => x.IdTipoCredito)
            .ToListAsync(cancellationToken);

        return ids.All(id => existingIds.Contains(id));
    }
}

internal class SaveTiposCreditoByIdLineaCreditoCommandHandler(
    IApplicationDbContext context,IValidator<SaveTiposCreditoByIdLineaCreditoCommand> validator
) : ICommandHandler<SaveTiposCreditoByIdLineaCreditoCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IValidator<SaveTiposCreditoByIdLineaCreditoCommand> _validator = validator;

    public async Task<Result> HandleAsync(SaveTiposCreditoByIdLineaCreditoCommand message, CancellationToken cancellationToken = default)
    {

        try
        {

            var validationResult = await _validator.ValidateAsync(message, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var existingRels = await _context.PSV_RelLineaCreditoTipoCredito
                .Where(x => x.IdLineaCredito == message.IdLineaCredito)
                .ToListAsync(cancellationToken);


            var model = message.Model;

            for (int i = 0; i < model.Count; i++)
            {
                var item = model[i];
                var rel = existingRels.FirstOrDefault(x => x.IdTipoCredito == item.IdTipoCredito);
                if (rel == null)
                {
                    rel = new PSV_RelLineaCreditoTipoCredito
                    {
                        IdLineaCredito = message.IdLineaCredito,
                        IdTipoCredito = item.IdTipoCredito,
                        Seleccionado = item.Seleccionado,
                    };
                    _context.PSV_RelLineaCreditoTipoCredito.Add(rel);
                }
                else
                {
                    rel.Seleccionado = item.Seleccionado;
                    _context.PSV_RelLineaCreditoTipoCredito.Update(rel);
                }

            }

            var idsInModel = model.Select(x => x.IdTipoCredito).ToList();
            var toDelete = existingRels
                .Where(x => !idsInModel.Contains(x.IdTipoCredito))
                .ToList();

            if (toDelete.Any())
            {
                _context.PSV_RelLineaCreditoTipoCredito.RemoveRange(toDelete);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}