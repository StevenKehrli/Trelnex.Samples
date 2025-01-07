using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trelnex.Core.Api.CommandProviders;
using Trelnex.Core.Api.Serilog;
using Trelnex.Core.Data;

namespace Trelnex.Integration.Tests.InMemory;

internal class InMemoryCommandProviders
{
    private readonly IServiceProvider _serviceProvider;

    private InMemoryCommandProviders(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static InMemoryCommandProviders Create(
        Action<ICommandProviderOptions> configureCommandProviders)
    {
        // create the service collection
        var services = new ServiceCollection();

        // create an empty configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var bootstrapLogger = services.AddSerilog(
            configuration,
            "Trelnex.Integration.Tests");

        // create the command providers
        services.AddInMemoryCommandProviders(
            configuration,
            bootstrapLogger,
            configureCommandProviders);

        var serviceProvider = services.BuildServiceProvider();

        return new InMemoryCommandProviders(serviceProvider);
    }

    public ICommandProvider<TInterface> Get<TInterface>()
        where TInterface : class, IBaseItem
    {
        return _serviceProvider.GetRequiredService<ICommandProvider<TInterface>>();
    }
}
