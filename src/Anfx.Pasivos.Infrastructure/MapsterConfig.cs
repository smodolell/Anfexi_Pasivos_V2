using Mapster;

namespace Anfx.Pasivos.Infrastructure;

public static class MapsterConfig
{
    public static void Configure()
    {
        ConfigureGlobalSettings();


        TypeAdapterConfig.GlobalSettings.Scan(
            typeof(MapsterConfig).Assembly);


    }

    private static void ConfigureGlobalSettings()
    {
        // ✅ CONFIGURACIÓN GLOBAL RECOMENDADA
        TypeAdapterConfig.GlobalSettings.Default
            .IgnoreNullValues(true)           // Ignorar propiedades nulas
            .PreserveReference(true)          // Preservar referencias (evita loops)
            .ShallowCopyForSameType(true)     // Copia superficial para mismos tipos
            .MaxDepth(5)                      // Profundidad máxima para evitar loops
            .EnumMappingStrategy(EnumMappingStrategy.ByName); // Mapeo de enums por nombre

        // ✅ Configuración para colecciones
        TypeAdapterConfig.GlobalSettings.Default
            .AddDestinationTransform((string x) => x.Trim()) // Trim automático para strings
            .AddDestinationTransform((decimal x) => Math.Round(x, 2)); // Redondear decimales
    }
}