using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using WebGallery.Api.Helpers;
using WebGallery.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

JsonConvert.DefaultSettings = StartupHelper.ConfigureJsonSerializerSettings;

builder.Services.AddDbContext<DatabaseContext>(opts =>
    opts.ConfigureDatabase(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetService<DatabaseContext>();
    await db?.Database.MigrateAsync()!;
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
