using Application.Settings;
using Infrastructure.Configuration;
using Infrastructure.Persistence.Serializers;
using MongoDB.Bson.Serialization;

namespace API.Extensions;

public static class ConfigurationSettingExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));
        services.Configure<McpServerOptions>(configuration.GetSection("McpServer"));

        // BSON serializers are registered once, globally, on the static driver registry
        BsonSerializer.TryRegisterSerializer(new EmailSerializer());
        return services;
    }
}
