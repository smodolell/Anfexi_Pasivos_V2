using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class UpdateTasaVariableCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required TasaVariableDto Model { get; set; }
}

internal class UpdateTasaVariableCommandHandler : ICommandHandler<UpdateTasaVariableCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<TasaVariableDto> _validator;

    public UpdateTasaVariableCommandHandler(IApplicationDbContext context, IValidator<TasaVariableDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateTasaVariableCommand request, CancellationToken cancellationToken = default)
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
                .SingleOrDefaultAsync(t => t.IdTasa == request.Id && t.EsVariable == true, cancellationToken);

            if (entity == null)
            {
                return Result.NotFound("Tasa variable no encontrada");
            }

            entity.Tasa1 = model.Nombre;

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
