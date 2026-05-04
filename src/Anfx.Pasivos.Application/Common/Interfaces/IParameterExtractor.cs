namespace Anfx.Pasivos.Application.Common.Interfaces;
/// <summary>
/// Estrategia para extraer parámetros de un stored procedure
/// </summary>
public interface IParameterExtractor
{
    /// <summary>
    /// Extrae la definición de parámetros de un stored procedure
    /// </summary>
    Task<IReadOnlyList<ParameterDefinitionDto>> ExtractAsync(
        string storedProcedure,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indica si esta estrategia puede manejar el proveedor de BD actual
    /// </summary>
    bool CanHandle(DbProvider provider);
}

/// <summary>
/// Definición de un parámetro extraído del SP
/// </summary>
public class ParameterDefinitionDto
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public ParameterDirection Direction { get; set; }
    public int? Length { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public int Order { get; set; }
}

public enum ParameterDirection
{
    Input,
    Output,
    InputOutput,
    ReturnValue
}

public enum DbProvider
{
    SqlServer,
    Oracle,
    PostgreSQL,
    MySql,
    SQLite
}

