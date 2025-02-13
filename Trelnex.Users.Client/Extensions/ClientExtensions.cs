using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trelnex.Core.Client;

namespace Trelnex.Users.Client;

/// <summary>
/// Extension methods to add the <see cref="IUsersClient"/> to the <see cref="IServiceCollection"/>.
/// </summary>
public static class ClientExtensions
{
    /// <summary>
    /// Add the <see cref="IUsersClient"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithUsersClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddClient<IUsersClient, UsersClient>(
            configuration: configuration);
    }
}
