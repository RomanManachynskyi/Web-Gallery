using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using NodaTime;
using Newtonsoft.Json.Converters;
using NodaTime.Serialization.JsonNet;

namespace WebGallery.Api.Helpers;

public static class StartupHelper
{
    public static void AddSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("0.00:00:00")
            });
            options.ConfigureForNodaTime();
        });
    }

    public static JsonSerializerSettings ConfigureJsonSerializerSettings()
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        return settings;
    }

    public static void ConfigureDatabase(this DbContextOptionsBuilder options, string connectionString)
    {
        options.UseNpgsql(connectionString, opts =>
        {
            opts.UseNodaTime();
            opts.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);
        });
    }
}
