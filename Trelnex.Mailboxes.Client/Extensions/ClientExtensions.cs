using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trelnex.Client.Common;

namespace Trelnex.Mailboxes.Client;

/// <summary>
/// Extension methods to add the <see cref="IMailboxesClient"/> to the <see cref="IServiceCollection"/>.
/// </summary>
public static class ClientExtensions
{
    /// <summary>
    /// Add the <see cref="IMailboxesClient"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithMailboxesClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.WithCommonClient<IMailboxesClient, MailboxesClient>(
            configuration: configuration,
            clientName: MailboxesClient.Name);
    }
}
