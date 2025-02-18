using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Subway.Mvp.Apis.FreshMenu;
using Subway.Mvp.Application.Features.FreshMenu;
using Testcontainers.RavenDb;

namespace Subway.Mvp.Tests;
public class FreshMenuIntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RavenDbContainer _container = new RavenDbBuilder()
        .WithImage("ravendb/ravendb:latest")
        //.WithName("ravendb-test")
        //.WithHostname("ravendb-test")
        .WithPortBinding(51500, 8080)
        .WithPortBinding(38889, 38888)
        .WithEnvironment("RAVEN_Security_UnsecuredAccessAllowed", "PrivateNetwork")
        .WithEnvironment("RAVEN_Setup_Mode", "Unsecured")
        .WithEnvironment("RAVEN_License_Eula_Accepted", "true")
        .WithExposedPort("51500")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ServiceDescriptor? descriptor =
                services.SingleOrDefault(s => s.ServiceType == typeof(FreshMenuStorageOptions));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.ConfigureOptions<FreshMenuStorageSetup>();
            services.AddSingleton(new FreshMenuStorageOptions()
            {
                DatabaseName = "FreshMenuDb",
                DataDirectory = "./.containers/freshmenu/v1",
                ServerUrl = "http://localhost:51500"
            });
        });
    }

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _container.StopAsync();
    }
}
