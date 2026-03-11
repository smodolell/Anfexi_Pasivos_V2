using Anfx.Pasivos.Application.Features.Configuracion.Dtos;
using Azure.Core;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class CreateFondeadorCommand : ICommand<Result<int>>
{

    public required FondeadorEditDto Model { get; set; }
}


internal class CreateFondeadorCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<FondeadorEditDto> validator) : ICommandHandler<CreateFondeadorCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<FondeadorEditDto> _validator = validator;

    public async Task<Result<int>> HandleAsync(CreateFondeadorCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_Fondeador>(model);
            await _context.PSV_Fondeador.AddAsync(entity);

            await _context.SaveChangesAsync(cancellationToken);


            return Result.Created(entity.IdFondeador);

        }
        catch (Exception ex)
        {

            return Result.Error(ex.Message);

        }
    }
}