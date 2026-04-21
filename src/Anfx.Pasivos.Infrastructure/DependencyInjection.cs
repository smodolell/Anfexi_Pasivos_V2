using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Infrastructure.Persistence;
using Anfx.Pasivos.Infrastructure.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anfx.Pasivos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuración de Mapster
        MapsterConfig.Configure();

        services.AddMapster();


        // 2. Configuración de Base de Datos
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Anfx.Pasivos.Infrastructure"));
        }, ServiceLifetime.Scoped);

        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Scoped);

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IConsecutivoService, ConsecutivoService>();
        services.AddScoped<IPaginator, Paginator>();
        services.AddScoped<IDynamicSorter, DynamicSorter>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IParameterExtractor, SqlServerParameterExtractor>();


        return services;
    }

}