using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using WebGallery.Api.Helpers;
using WebGallery.Api.Middleware;
using WebGallery.Core;
using WebGallery.Core.Service;
using WebGallery.Data;
using WebGallery.Data.Repositories;
using WebGallery.Shared.AWS.Cognito;
using WebGallery.Shared.AWS.IAM;
using WebGallery.Shared.AWS.S3;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

JsonConvert.DefaultSettings = StartupHelper.ConfigureJsonSerializerSettings;

builder.Services.AddDbContext<DatabaseContext>(opts =>
    opts.ConfigureDatabase(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddHttpContextAccessor();

#region Build Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IErrorResponseFormatter, ErrorResponseFormatter>();

builder.Services.AddSingleton(typeof(IIamConfig), builder.Configuration.GetSection("AWS:IAM").Get<IamConfig>());
builder.Services.AddSingleton(typeof(IS3Config), builder.Configuration.GetSection("AWS:S3").Get<S3Config>());
builder.Services.AddSingleton(typeof(ICognitoConfig), builder.Configuration.GetSection("AWS:Cognito").Get<CognitoConfig>());
builder.Services.AddScoped<ICognitoService, CognitoService>();
builder.Services.AddScoped<IS3Service, S3Service>();

builder.Services.AddScoped<IUserProfilesService, UserProfilesService>();
builder.Services.AddScoped<IArtworksService, ArtworksService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

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

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
