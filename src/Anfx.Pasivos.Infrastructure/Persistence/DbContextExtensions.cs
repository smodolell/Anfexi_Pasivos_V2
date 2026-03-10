#nullable disable
using Microsoft.EntityFrameworkCore;

namespace Anfx.Pasivos.Infrastructure.Persistence;

public static class DbContextExtensions
{
    public static async Task<List<T>> SqlQueryAsync<T>(this DbContext db, string sql, object[] parameters = null, CancellationToken? cancellationToken = default)
       where T : class
    {
        parameters ??= Array.Empty<object>();
        cancellationToken ??= CancellationToken.None;
        
        if (typeof(T).GetProperties().Any())
        {
            return await db.Database
                .SqlQueryRaw<T>(sql, parameters)
                .ToListAsync(cancellationToken.Value);
        }
        else
        {
            await db.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken.Value);
            return default;
        }
    }
}
