using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Anfx.Pasivos.Infrastructure.Services;

/// <summary>
/// Estrategia nativa para extraer parámetros de SPs en SQL Server
/// </summary>
public class SqlServerParameterExtractor : IParameterExtractor
{
    private readonly ApplicationDbContext _context;

    public SqlServerParameterExtractor(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool CanHandle(DbProvider provider) => provider == DbProvider.SqlServer;

    public async Task<IReadOnlyList<ParameterDefinitionDto>> ExtractAsync(
        string storedProcedure,
        CancellationToken cancellationToken = default)
    {
        var parameters = new List<ParameterDefinitionDto>();

        // Opción 1: Usar INFORMATION_SCHEMA.PARAMETERS
        await using var command = _context.Database.GetDbConnection().CreateCommand();

        command.CommandText = @"
            SELECT 
                p.PARAMETER_NAME,
                p.DATA_TYPE,
                p.ORDINAL_POSITION,
                CASE 
                    WHEN p.PARAMETER_MODE = 'IN' THEN 0
                    WHEN p.PARAMETER_MODE = 'OUT' THEN 1
                    WHEN p.PARAMETER_MODE = 'INOUT' THEN 2
                    ELSE 0
                END AS PARAMETER_DIRECTION,
                CAST(p.CHARACTER_MAXIMUM_LENGTH AS INT) CHARACTER_MAXIMUM_LENGTH,
                CAST( p.NUMERIC_PRECISION AS INT) NUMERIC_PRECISION,
                CAST( p.NUMERIC_SCALE AS INT) NUMERIC_SCALE,
                ISNULL(t.is_output, 0) AS IS_OUTPUT
            FROM INFORMATION_SCHEMA.PARAMETERS p
            LEFT JOIN (
                SELECT 
                    name,
                    is_output,
                    parameter_id
                FROM sys.parameters 
                WHERE object_id = OBJECT_ID(@StoredProcedure)
            ) t ON t.name = p.PARAMETER_NAME
            WHERE p.SPECIFIC_NAME = @StoredProcedure
              AND p.SPECIFIC_SCHEMA = SCHEMA_NAME()
            ORDER BY p.ORDINAL_POSITION";

        command.Parameters.Add(new SqlParameter("@StoredProcedure", storedProcedure));

        var wasClosed = command.Connection?.State == ConnectionState.Closed;

        try
        {
            if (wasClosed)
                await command.Connection!.OpenAsync(cancellationToken);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var parameter = new ParameterDefinitionDto
                {
                    Name = reader.GetString(0),  // PARAMETER_NAME
                    DataType = reader.GetString(1),  // DATA_TYPE
                    Order = reader.GetInt32(2), // ORDINAL_POSITION
                    Direction = MapDirection(reader)
                };

                // Longitud (para varchar, nvarchar, etc.)
                if (!reader.IsDBNull(4))
                    parameter.Length = reader.GetInt32(4);

                // Precisión y escala (para decimal, numeric)
                if (!reader.IsDBNull(5))
                    parameter.Precision = reader.GetInt32(5);

                if (!reader.IsDBNull(6))
                    parameter.Scale = reader.GetInt32(6);

                parameters.Add(parameter);
            }
        }
        finally
        {
            if (wasClosed && command.Connection?.State != ConnectionState.Closed)
                await command.Connection!.CloseAsync();
        }

        return parameters;
    }

    private Application.Common.Interfaces.ParameterDirection MapDirection(System.Data.Common.DbDataReader reader)
    {
        // Método 1: Usar PARAMETER_MODE de INFORMATION_SCHEMA
        var mode = reader.GetInt32(3);

        if (mode == 1)
            return Application.Common.Interfaces.ParameterDirection.Output;

        if (mode == 2)
            return Application.Common.Interfaces.ParameterDirection.InputOutput;

        // Método 2: Verificar columna IS_OUTPUT de sys.parameters
        var isOutput = reader.GetBoolean(7);
        if (isOutput)
            return Application.Common.Interfaces.ParameterDirection.Output;

        return Application.Common.Interfaces.ParameterDirection.Input;
    }
}