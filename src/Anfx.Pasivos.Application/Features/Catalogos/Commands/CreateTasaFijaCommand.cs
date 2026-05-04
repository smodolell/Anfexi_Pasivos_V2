using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class CreateTasaFijaCommand : ICommand<Result<int>>
{
    public required TasaFijaDto Model { get; set; }
}

internal class CreateTasaFijaCommandHandler : ICommandHandler<CreateTasaFijaCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<TasaFijaDto> _validator;

    public CreateTasaFijaCommandHandler(IApplicationDbContext context, IValidator<TasaFijaDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateTasaFijaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = new Tasa
            {
                Tasa1 = model.Nombre,
                ValorTasa = model.ValorTasa,
                FecTasa = model.FecTasa,
                EsVariable = false,
                Activo = true
            };

            await _context.Tasa.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdTasa);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
