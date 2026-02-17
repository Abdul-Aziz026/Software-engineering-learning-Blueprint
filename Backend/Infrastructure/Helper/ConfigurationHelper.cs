using Microsoft.Extensions.Configuration;
using System;

namespace Infrastructure.Helper;

public class ConfigurationHelper
{
    private static IConfiguration _configuration =  default!;
    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string GetConfigurationValue(string key)
    {
        return _configuration[key] ?? throw new ArgumentNullException($"Configuration key '{key}' not found.");
    }
}
