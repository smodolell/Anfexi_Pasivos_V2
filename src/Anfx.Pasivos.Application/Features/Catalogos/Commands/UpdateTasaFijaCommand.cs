using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class UpdateTasaFijaCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required TasaFijaDto Model { get; set; }
}

internal class UpdateTasaFijaCommandHandler : ICommandHandler<UpdateTasaFijaCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<TasaFijaDto> _validator;

    public UpdateTasaFijaCommandHandler(IApplicationDbContext context, IValidator<TasaFijaDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateTasaFijaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.Tasa
                .SingleOrDefaultAsync(t => t.IdTasa == request.Id && t.EsVariable == false, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Tasa fija no encontrada");
            }

            entity.Tasa1 = model.Nombre;
            entity.ValorTasa = model.ValorTasa;
            entity.FecTasa = model.FecTasa;

            _context.Tasa.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
