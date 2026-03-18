#nullable disable
using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Common.Models.StoredProcedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Anfx.Pasivos.Infrastructure.Persistence;

public partial class ApplicationDbContext
{
    private IApplicationDbContextProcedures _procedures;

    public virtual IApplicationDbContextProcedures Procedures
    {
        get
        {
            if (_procedures is null) _procedures = new ApplicationDbContextProcedures(this);
            return _procedures;
        }
        set
        {
            _procedures = value;
        }
    }

    public IApplicationDbContextProcedures GetProcedures()
    {
        return Procedures;
    }
}

public partial class ApplicationDbContextProcedures : IApplicationDbContextProcedures
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextProcedures(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual async Task<List<usp_CarteraActiva_CIResult>> usp_CarteraActiva_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new []
        {
            new SqlParameter
            {
                ParameterName = "IdFondeador",
                Value = idFondeador ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "IdContratoPasivo",
                Value = idContratoPasivo ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "IdContratoActivo",
                Value = idContratoActivo ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "Saldos",
                Value = saldos ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            parameterreturnValue,
        };
        var _ = await _context.SqlQueryAsync<usp_CarteraActiva_CIResult>("EXEC @returnValue = [dbo].[usp_CarteraActiva_CI] @IdFondeador = @IdFondeador, @IdContratoPasivo = @IdContratoPasivo, @IdContratoActivo = @IdContratoActivo, @Saldos = @Saldos", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_CarteraActivaMensual_CIResult>> usp_CarteraActivaMensual_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new []
        {
            new SqlParameter
            {
                ParameterName = "IdFondeador",
                Value = idFondeador ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "IdContratoPasivo",
                Value = idContratoPasivo ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "IdContratoActivo",
                Value = idContratoActivo ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "Saldos",
                Value = saldos ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            parameterreturnValue,
        };
        var _ = await _context.SqlQueryAsync<usp_CarteraActivaMensual_CIResult>("EXEC @returnValue = [dbo].[usp_CarteraActivaMensual_CI] @IdFondeador = @IdFondeador, @IdContratoPasivo = @IdContratoPasivo, @IdContratoActivo = @IdContratoActivo, @Saldos = @Saldos", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_CarteraPasivaMensual_CIResult>> usp_CarteraPasivaMensual_CIAsync(int? idFondeador, int? idContratoPasivo, int? idContratoActivo, int? saldos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new []
        {
            new SqlParameter
            {
                ParameterName = "IdFondeador",
                Value = idFondeador ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "IdContratoPasivo",
                Value = idContratoPasivo ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "IdContratoActivo",
                Value = idContratoActivo ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            new SqlParameter
            {
                ParameterName = "Saldos",
                Value = saldos ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.Int,
            },
            parameterreturnValue,
        };
        var _ = await _context.SqlQueryAsync<usp_CarteraPasivaMensual_CIResult>("EXEC @returnValue = [dbo].[usp_CarteraPasivaMensual_CI] @IdFondeador = @IdFondeador, @IdContratoPasivo = @IdContratoPasivo, @IdContratoActivo = @IdContratoActivo, @Saldos = @Saldos", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }


    public virtual async Task<List<usp_PSV_AnticipoACapitalResult>> usp_PSV_AnticipoACapitalAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "IdTerminacion",
                    Value = idTerminacion ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                parameterreturnValue,
            };
        var _ = await _context.SqlQueryAsync<usp_PSV_AnticipoACapitalResult>("EXEC @returnValue = [dbo].[usp_PSV_AnticipoACapital] @IdTerminacion = @IdTerminacion", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_PSV_AplicaAnticipo_CIResult>> usp_PSV_AplicaAnticipo_CIAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "IdTerminacion",
                    Value = idTerminacion ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                parameterreturnValue,
            };
        var _ = await _context.SqlQueryAsync<usp_PSV_AplicaAnticipo_CIResult>("EXEC @returnValue = [dbo].[usp_PSV_AplicaAnticipo_CI] @IdTerminacion = @IdTerminacion", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_PSV_LiquidacionResult>> usp_PSV_LiquidacionAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "IdTerminacion",
                    Value = idTerminacion ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                parameterreturnValue,
            };
        var _ = await _context.SqlQueryAsync<usp_PSV_LiquidacionResult>("EXEC @returnValue = [dbo].[usp_PSV_Liquidacion] @IdTerminacion = @IdTerminacion", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<int> usp_PSV_AnticipoAPlazoAsync(int? idTerminacion, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "IdTerminacion",
                    Value = idTerminacion ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                parameterreturnValue,
            };
        var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [dbo].[usp_PSV_AnticipoAPlazo] @IdTerminacion = @IdTerminacion", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_PSV_CalculaInteresResult>> usp_PSV_CalculaInteresAsync(int? idContrato, DateOnly? fechaCorte, decimal? montoAnticipo, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "IdContrato",
                    Value = idContrato ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "FechaCorte",
                    Value = fechaCorte ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Date,
                },
                new SqlParameter
                {
                    ParameterName = "MontoAnticipo",
                    Precision = 13,
                    Scale = 2,
                    Value = montoAnticipo ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Decimal,
                },
                parameterreturnValue,
            };
        var _ = await _context.SqlQueryAsync<usp_PSV_CalculaInteresResult>("EXEC @returnValue = [dbo].[usp_PSV_CalculaInteres] @IdContrato = @IdContrato, @FechaCorte = @FechaCorte, @MontoAnticipo = @MontoAnticipo", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_PSV_PagarCargosResult>> usp_PSV_PagarCargosAsync(int? idFondeador, int? idContrato, int? idUsuario, int? idTipoPago, int? idCuentaBancaria, DateOnly? fechaPago, string referencia, decimal? montoPago, IEnumerable<KeyItem> movimientos, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "IdFondeador",
                    Value = idFondeador ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "IdContrato",
                    Value = idContrato ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "IdUsuario",
                    Value = idUsuario ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "IdTipoPago",
                    Value = idTipoPago ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "IdCuentaBancaria",
                    Value = idCuentaBancaria ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "FechaPago",
                    Value = fechaPago ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Date,
                },
                new SqlParameter
                {
                    ParameterName = "Referencia",
                    Size = 100,
                    Value = referencia ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                new SqlParameter
                {
                    ParameterName = "MontoPago",
                    Precision = 18,
                    Scale = 2,
                    Value = montoPago ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Decimal,
                },
                new SqlParameter
                {
                    ParameterName = "Movimientos",
                    Value = movimientos == null ? Convert.DBNull : ToDataTable(movimientos) ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Structured,
                    TypeName = "[dbo].[KeyItem]",
                },
                parameterreturnValue,
            };
        var _ = await _context.SqlQueryAsync<usp_PSV_PagarCargosResult>("EXEC @returnValue = [dbo].[usp_PSV_PagarCargos] @IdFondeador = @IdFondeador, @IdContrato = @IdContrato, @IdUsuario = @IdUsuario, @IdTipoPago = @IdTipoPago, @IdCuentaBancaria = @IdCuentaBancaria, @FechaPago = @FechaPago, @Referencia = @Referencia, @MontoPago = @MontoPago, @Movimientos = @Movimientos", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_PSV_ProcesaMoratoriosResult>> usp_PSV_ProcesaMoratoriosAsync(DateTime? fechaProcesamiento, int? idContrato, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "FechaProcesamiento",
                    Value = fechaProcesamiento ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.DateTime,
                },
                new SqlParameter
                {
                    ParameterName = "IdContrato",
                    Value = idContrato ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                parameterreturnValue,
            };
        var _ = await _context.SqlQueryAsync<usp_PSV_ProcesaMoratoriosResult>("EXEC @returnValue = [dbo].[usp_PSV_ProcesaMoratorios] @FechaProcesamiento = @FechaProcesamiento, @IdContrato = @IdContrato", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    public virtual async Task<List<usp_PSV_ProcesaVencimientosResult>> usp_PSV_ProcesaVencimientosAsync(int? idFondeador, int? idContrato, DateOnly? fechaInicial, DateOnly? fechaFinal, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    {
        var parameterreturnValue = new SqlParameter
        {
            ParameterName = "returnValue",
            Direction = System.Data.ParameterDirection.Output,
            SqlDbType = System.Data.SqlDbType.Int,
        };

        var sqlParameters = new[]
        {
                new SqlParameter
                {
                    ParameterName = "IdFondeador",
                    Value = idFondeador ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "IdContrato",
                    Value = idContrato ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "FechaInicial",
                    Value = fechaInicial ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Date,
                },
                new SqlParameter
                {
                    ParameterName = "FechaFinal",
                    Value = fechaFinal ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Date,
                },
                parameterreturnValue,
            };
        var _ = await _context.SqlQueryAsync<usp_PSV_ProcesaVencimientosResult>("EXEC @returnValue = [dbo].[usp_PSV_ProcesaVencimientos] @IdFondeador = @IdFondeador, @IdContrato = @IdContrato, @FechaInicial = @FechaInicial, @FechaFinal = @FechaFinal", sqlParameters, cancellationToken);

        returnValue?.SetValue(parameterreturnValue.Value);

        return _;
    }

    private static DataTable ToDataTable<T>(IEnumerable<T> items)
    {
        var dataTable = new DataTable();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            var propertyType = prop.PropertyType;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }
            dataTable.Columns.Add(prop.Name, propertyType);
        }

        foreach (var item in items)
        {
            var values = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                values[i] = properties[i].GetValue(item) ?? DBNull.Value;
            }
            dataTable.Rows.Add(values);
        }

        return dataTable;
    }
}
