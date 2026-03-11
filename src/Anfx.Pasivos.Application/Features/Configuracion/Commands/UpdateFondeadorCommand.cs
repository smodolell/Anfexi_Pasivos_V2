using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;


public class UpdateFondeadorCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required FondeadorEditDto Model { get; set; }
}

internal class UpdateFondeadorCommandHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<FondeadorEditDto> validator
) : ICommandHandler<UpdateFondeadorCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<FondeadorEditDto> _validator = validator;

    public async Task<Result> HandleAsync(UpdateFondeadorCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var fondeador = await _context.PSV_Fondeador.SingleOrDefaultAsync(x => x.IdFondeador == request.Id);
            if (fondeador == null)
            {
                return Result.NotFound("Fondeador no encontrado");
            }

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            _mapper.Map(model, fondeador);
            _context.PSV_Fondeador.Update(fondeador);
            await _context.SaveChangesAsync(cancellationToken);


            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}