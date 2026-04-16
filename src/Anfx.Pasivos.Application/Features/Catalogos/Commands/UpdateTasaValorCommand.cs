using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class UpdateTasaValorCommand : ICommand<Result>
{
    public int IdTasaValor { get; set; }
    public required TasaValorDto Model { get; set; }
}

internal class UpdateTasaValorCommandHandler : ICommandHandler<UpdateTasaValorCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<TasaValorDto> _validator;

    public UpdateTasaValorCommandHandler(IApplicationDbContext context, IValidator<TasaValorDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateTasaValorCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.TasaValor
                .SingleOrDefaultAsync(tv => tv.IdTasaValor == request.IdTasaValor, cancellationToken);

            if (entity == null)
            {
                return Result.NotFound("Valor de tasa no encontrado");
            }

            entity.ValorTasa = model.ValorTasa;
            entity.FecValorTasa = model.FecValorTasa;
            entity.FecRegistroTasa = model.FecRegistroTasa;

            _context.TasaValor.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var ultimoValorTasa = await _context.TasaValor
                .Where(tv => tv.IdTasa == entity.IdTasa)
                .OrderByDescending(tv => tv.FecValorTasa) // Ordenar por fecha de valor
                    .ThenByDescending(tv => tv.FecRegistroTasa) // En caso de misma fecha, por registro
                .FirstOrDefaultAsync(cancellationToken);

            if (ultimoValorTasa != null)
            {
                var tasa = await _context.Tasa
                    .SingleOrDefaultAsync(r => r.IdTasa == entity.IdTasa, cancellationToken);

                if (tasa != null)
                {
                    tasa.ValorTasa = ultimoValorTasa.ValorTasa;
                    tasa.FecTasa = DateTime.Now;

                    _context.Tasa.Update(tasa);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }



            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
