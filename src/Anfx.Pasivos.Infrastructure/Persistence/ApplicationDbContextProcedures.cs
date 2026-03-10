#nullable disable
using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Common.Models.StoredProcedures;
using Microsoft.Data.SqlClient;

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
}
