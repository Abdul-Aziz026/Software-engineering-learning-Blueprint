using Application.Settings;

namespace API.Extensions;

public static class ConfigurationSettingExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));
        return services;
    }
}
