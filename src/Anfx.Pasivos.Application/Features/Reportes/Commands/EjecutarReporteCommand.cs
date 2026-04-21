using Anfx.Pasivos.Application.Features.Reportes.DTOs;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using System.Text.Json;
using Anfx.Pasivos.Application.Common;

namespace Anfx.Pasivos.Application.Features.Reportes.Commands;

public class EjecutarReporteCommand : ICommand<Result<ReporteExportDto>>
{
    public required ReporteExecuteDto Model { get; set; }
    public bool GuardarArchivo { get; set; }
}

public class EjecutarReporteCommandHandler(
    IApplicationDbContext context,
    IConfiguration configuration
) : ICommandHandler<EjecutarReporteCommand, Result<ReporteExportDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    public async Task<Result<ReporteExportDto>> HandleAsync(EjecutarReporteCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = message.Model;
            var oReporte = await _context.RSP_Reporte
                .SingleOrDefaultAsync(r => r.Id == model.ReporteId, cancellationToken);

            if (oReporte == null)
                return Result.NotFound("El reporte no existe.");

            var dt = EjecutarStoredProcedure(oReporte.StoredProcedure, model);

            var exportDto = new ReporteExportDto { FileName = model.NomReporte };

            if (model.ReporteFormatoId == ApplicationConstants.ReporteFormatoId_Excel)
            {
                exportDto.Data = ExportarExcel(dt);
                exportDto.ContentType = ApplicationConstants.ContentType_Excel;
                exportDto.FileName += ".xlsx";
            }
            else
            {
                exportDto.Data = ExportarTexto(dt);
                exportDto.ContentType = ApplicationConstants.ContentType_Txt;
                exportDto.FileName += ".txt";
            }

            if (message.GuardarArchivo)
                await GuardarArchivoAsync(exportDto, oReporte.Id, model, cancellationToken);

            return Result.Success(exportDto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    private async Task GuardarArchivoAsync(ReporteExportDto exportDto, int reporteId, ReporteExecuteDto model, CancellationToken cancellationToken)
    {
        var basePath = _configuration["Archivos:BasePath"] ?? Path.Combine(AppContext.BaseDirectory, "archivos-reporte");
        Directory.CreateDirectory(basePath);

        var extension = exportDto.FileName.EndsWith(".xlsx") ? ".xlsx" : ".txt";
        var nombreUnico = $"{Guid.NewGuid()}{extension}";
        var mapPath = Path.Combine(basePath, nombreUnico);

        await File.WriteAllBytesAsync(mapPath, exportDto.Data, cancellationToken);

        var logParams = JsonSerializer.Serialize(model.Parametros?.Select(p => new { p.NomParametro, p.Value }));

        var archivo = new RSP_Archivo
        {
            Id = Guid.NewGuid(),
            ReporteId = reporteId,
            FechaCreacion = DateTime.Now,
            NombreArchivo = exportDto.FileName,
            NombreUnico = nombreUnico,
            ContentType = exportDto.ContentType,
            Extension = extension,
            MapPath = mapPath,
            LogParameters = logParams
        };

        _context.RSP_Archivo.Add(archivo);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private DataTable EjecutarStoredProcedure(string storedProcedure, ReporteExecuteDto model)
    {
        var dt = new DataTable();
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using var connection = new SqlConnection(connectionString);
        var da = new SqlDataAdapter(storedProcedure, connection);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.CommandTimeout = 1200;

        model.Parametros ??= [];
        foreach (var param in model.Parametros)
        {
            if (param.InputId == ApplicationConstants.RSP_Input_UsuarioActivoId)
                param.Value = model.UserId;

            switch (param.TipoDato)
            {
                case "datetime":
                case "datetime2":
                    da.SelectCommand.Parameters.Add(param.NomParametro, SqlDbType.DateTime);
                    try { da.SelectCommand.Parameters[param.NomParametro].Value = param.ValueDateTime; }
                    catch { da.SelectCommand.Parameters[param.NomParametro].Value = DBNull.Value; }
                    break;

                case "date":
                    da.SelectCommand.Parameters.Add(param.NomParametro, SqlDbType.Date);
                    try { da.SelectCommand.Parameters[param.NomParametro].Value = param.ValueDateTime; }
                    catch { da.SelectCommand.Parameters[param.NomParametro].Value = DBNull.Value; }
                    break;

                case "varchar":
                case "nvarchar":
                    da.SelectCommand.Parameters.Add(param.NomParametro, SqlDbType.VarChar);
                    da.SelectCommand.Parameters[param.NomParametro].Value = param.Value ?? "";
                    break;

                case "int":
                case "tinyint":
                case "bigint":
                    da.SelectCommand.Parameters.Add(param.NomParametro, SqlDbType.Int);
                    try { da.SelectCommand.Parameters[param.NomParametro].Value = Convert.ToInt32(param.Value ?? "0"); }
                    catch { da.SelectCommand.Parameters[param.NomParametro].Value = 0; }
                    break;

                case "decimal":
                    da.SelectCommand.Parameters.Add(param.NomParametro, SqlDbType.Decimal);
                    try { da.SelectCommand.Parameters[param.NomParametro].Value = Convert.ToDecimal(param.Value); }
                    catch { da.SelectCommand.Parameters[param.NomParametro].Value = 0.0m; }
                    break;

                case "bit":
                    da.SelectCommand.Parameters.Add(param.NomParametro, SqlDbType.Bit);
                    da.SelectCommand.Parameters[param.NomParametro].Value = param.ValueBoolean;
                    break;
            }
        }

        da.Fill(dt);
        return dt;
    }

    private static byte[] ExportarExcel(DataTable data)
    {
        data.TableName = "Reporte";
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(data);

        // Dar formato a la primera fila (encabezados)
        var headerRow = worksheet.FirstRow();
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Fill.BackgroundColor = XLColor.Blue;

        // Opcional: centrar el texto de los encabezados
        headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static byte[] ExportarTexto(DataTable data)
    {
        var sb = new StringBuilder();
        foreach (DataRow row in data.Rows)
        {
            var items = new List<string>();
            foreach (DataColumn col in data.Columns)
                items.Add(Convert.ToString(row[col.ColumnName]) ?? "");
            sb.AppendLine(string.Join(";", items));
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
