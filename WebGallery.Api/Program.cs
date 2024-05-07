using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using WebGallery.Api.Helpers;
using WebGallery.Core;
using WebGallery.Core.Service;
using WebGallery.Data;
using WebGallery.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

JsonConvert.DefaultSettings = StartupHelper.ConfigureJsonSerializerSettings;

builder.Services.AddDbContext<DatabaseContext>(opts =>
    opts.ConfigureDatabase(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IArtworksService, ArtworksService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
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
