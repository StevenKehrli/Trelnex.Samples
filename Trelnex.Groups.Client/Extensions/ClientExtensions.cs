using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trelnex.Client.Common;

namespace Trelnex.Groups.Client;

/// <summary>
/// Extension methods to add the <see cref="IGroupsClient"/> to the <see cref="IServiceCollection"/>.
/// </summary>
public static class ClientExtensions
{
    /// <summary>
    /// Add the <see cref="IGroupsClient"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithGroupsClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.WithCommonClient<IGroupsClient, GroupsClient>(
            configuration: configuration,
            clientName: GroupsClient.Name);
    }
}
