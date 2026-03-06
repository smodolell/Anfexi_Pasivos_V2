using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class UpdateBancoCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required BancoDto Model { get; set; }
}

internal class UpdateBancoCommandHandler : ICommandHandler<UpdateBancoCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<BancoDto> _validator;

    public UpdateBancoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<BancoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateBancoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.PSV_Banco.SingleOrDefaultAsync(r => r.IdBanco == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Banco no existe");
            }
            _mapper.Map(model, entity);

            _context.PSV_Banco.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
