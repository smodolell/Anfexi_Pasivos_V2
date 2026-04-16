using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class CreateTasaValorCommand : ICommand<Result<int>>
{
    public int IdTasa { get; set; }
    public required TasaValorDto Model { get; set; }
}

internal class CreateTasaValorCommandHandler : ICommandHandler<CreateTasaValorCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<TasaValorDto> _validator;

    public CreateTasaValorCommandHandler(IApplicationDbContext context, IValidator<TasaValorDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateTasaValorCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var tasaExiste = await _context.Tasa
                .AnyAsync(t => t.IdTasa == request.IdTasa && t.EsVariable == true, cancellationToken);

            if (!tasaExiste)
            {
                return Result.NotFound("Tasa variable no encontrada");
            }

            var entity = new TasaValor
            {
                IdTasa = request.IdTasa,
                ValorTasa = model.ValorTasa,
                FecValorTasa = model.FecValorTasa,
                FecRegistroTasa = model.FecRegistroTasa ?? DateTime.Now
            };

            await _context.TasaValor.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdTasaValor);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
