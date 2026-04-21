using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class DownloadArchivoQuery : IQuery<Result<ReporteExportDto>>
{
    public Guid ArchivoId { get; set; }
}

internal class DownloadArchivoQueryHandler(
    IApplicationDbContext context
) : IQueryHandler<DownloadArchivoQuery, Result<ReporteExportDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ReporteExportDto>> HandleAsync(DownloadArchivoQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var archivo = await _context.RSP_Archivo
                .SingleOrDefaultAsync(a => a.Id == request.ArchivoId, cancellationToken);

            if (archivo == null)
                return Result.NotFound("El archivo no existe.");

            if (!File.Exists(archivo.MapPath))
                return Result.Error("El archivo no se encontró en el disco.");

            var data = await File.ReadAllBytesAsync(archivo.MapPath, cancellationToken);

            return Result.Success(new ReporteExportDto
            {
                Data = data,
                ContentType = archivo.ContentType,
                FileName = archivo.NombreArchivo
            });
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
