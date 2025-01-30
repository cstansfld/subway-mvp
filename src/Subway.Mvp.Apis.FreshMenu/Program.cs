using System.Diagnostics;
using Asp.Versioning.Builder;
using Asp.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Serilog;
using Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints;
using Subway.Mvp.Apis.FreshMenu.Options;
using Subway.Mvp.Application;
using Subway.Mvp.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext());

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("Expire20secs", builder =>
        builder.Expire(TimeSpan.FromSeconds(20)));
    options.AddPolicy("Expire30secs", builder =>
        builder.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("Expire60secs", builder =>
        builder.Expire(TimeSpan.FromMinutes(1)));
    options.AddPolicy("Expire60mins", builder =>
        builder.Expire(TimeSpan.FromHours(1)));
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);

builder.Services
    .AddApplication()
    .AddInfrastructure();

builder.Services.AddHttpContextAccessor();

builder.Services.AddApiVersioning(options => {
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
}).AddApiExplorer(options => {
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(s => s.FullName?.Replace("+", ".")));
builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

builder.Services.AddExceptionHandler<Subway.Mvp.Application.Handlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app.MapGroup("v{apiVersion:apiVersion}").WithApiVersionSet(apiVersionSet);

versionedGroup.MapFreshMenuEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (string? groupName in app.DescribeApiVersions().Select(namedGroup => namedGroup.GroupName))
        {
            options.SwaggerEndpoint(
                url: $"/swagger/{groupName}/swagger.json",
                name: groupName.ToUpperInvariant());
        }
    });
}

app.UseResponseCompression();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication(); // Authentication before authorization
//app.UseAuthorization();

app.UseOutputCache();

app.AddHealthCheckApp();

app.UseExceptionHandler();

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Subway.Mvp.Apis.FreshMenu
{
    public partial class Program;
}
