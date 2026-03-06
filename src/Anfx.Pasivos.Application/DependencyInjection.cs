using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Queries;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Anfx.Pasivos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        TypeAdapterConfig.GlobalSettings.Scan(typeof(DependencyInjection).Assembly);
        TypeAdapterConfig.GlobalSettings.Compile();

        services.AddValidatorsFromAssemblyContaining<PasivosApplicationMarker>();

        services.AddLiteBus(configuration =>
        {
            var assembly = typeof(PasivosApplicationMarker).Assembly;

            configuration.AddCommandModule(m => m.RegisterFromAssembly(assembly));
            configuration.AddQueryModule(m => m.RegisterFromAssembly(assembly));

        });



        return services;
    }
}
class PasivosApplicationMarker { }