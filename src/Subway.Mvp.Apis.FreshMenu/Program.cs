using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Serilog;
using Subway.Mvp.Apis.FreshMenu.FreshMenuEndpoints;
using Subway.Mvp.Application;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext());

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("Expire20", builder =>
        builder.Expire(TimeSpan.FromSeconds(20)));
    options.AddPolicy("Expire30", builder =>
        builder.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("Expire60", builder =>
        builder.Expire(TimeSpan.FromSeconds(60)));
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
    .AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //options.CustomSchemaIds(type => type.ToString()); // this is kept if reverted
    options.CustomSchemaIds(s => s.FullName?.Replace("+", "."));
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "JSON FreshMenu.API", Version = "v1" });
});

builder.Services.AddExceptionHandler<Subway.Mvp.Application.Handlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication(); // Authentication before authorization
//app.UseAuthorization();

app.UseOutputCache();

app.AddHealthCheckApp();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

FreshMenuEndpoints.MapFreshMenuEndpoints(app);

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Subway.Mvp.Apis.FreshMenu
{
    public partial class Program;
}
