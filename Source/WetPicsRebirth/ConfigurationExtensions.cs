namespace WetPicsRebirth;

public static class ConfigurationExtensions
{
    public static T GetRequiredValue<T>(this IConfiguration configuration, string key) 
        => configuration.GetValue<T>(key) 
           ?? throw new Exception($"Value with key {key} is not found in configuration.");

    public static string GetRequiredConnectionString(this IConfiguration configuration, string name) 
        => configuration.GetConnectionString(name) 
           ?? throw new Exception($"Connection string with name {name} is not found.");
    
    public static T GetRequired<T>(this IConfiguration configuration)
        => configuration.Get<T>() ?? throw new Exception($"Can't get configuration object.");
}
