using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Semver;
using Trelnex.Core.Api.DataProviders;
using Trelnex.Core.Api.Configuration;
using Trelnex.Core.Api.Serilog;
using Trelnex.Core.Data;

namespace Trelnex.Integration.Tests.InMemory;

internal class InMemoryDataProviders
{
    private readonly IServiceProvider _serviceProvider;

    private InMemoryDataProviders(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static InMemoryDataProviders Create(
        Action<IDataProviderOptions> configureDataProviders)
    {
        // create the service collection
        var services = new ServiceCollection();

        // create an empty configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var bootstrapLogger = services.AddSerilog(
            configuration,
            new ServiceConfiguration() {
                FullName = "Trelnex.Integration.Tests",
                DisplayName = "Trelnex.Integration.Tests",
                Version = "0.0.0",
                Description = "Trelnex.Integration.Tests"
            });

        // create the data providers
        services.AddInMemoryDataProviders(
            configuration,
            bootstrapLogger,
            configureDataProviders);

        var serviceProvider = services.BuildServiceProvider();

        return new InMemoryDataProviders(serviceProvider);
    }

    public IDataProvider<TInterface> Get<TInterface>()
        where TInterface : class, IBaseItem
    {
        return _serviceProvider.GetRequiredService<IDataProvider<TInterface>>();
    }
}
