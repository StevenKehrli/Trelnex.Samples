using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trelnex.Core.Client;
using Trelnex.Core.Identity;

namespace Trelnex.Client.Common;

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
    /// <param name="clientName">The name of the client.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithCommonClient<IClient, TClient>(
        this IServiceCollection services,
        IConfiguration configuration,
        string clientName)
        where TClient : BaseClient, IClient
        where IClient : class
    {
        var clientConfiguration = configuration
            .GetSection("Clients")
            .GetSection(clientName)
            .Get<ClientConfiguration>()
            .ValidateOrThrow(clientName);

        services.AddHttpClient(clientName);

        // get the credential provider and access token provider
        var credentialProvider = services.GetCredentialProvider(clientConfiguration.CredentialProviderName);

        // get the access token provider
        var accessTokenProvider = credentialProvider.GetTokenProvider(
            credentialName: clientName,
            scope: clientConfiguration.Scope);

        services.AddKeyedSingleton(
            clientName,
            accessTokenProvider);

        services.AddKeyedSingleton(
            clientName,
            clientConfiguration.BaseUri);

        services.AddScoped<IClient, TClient>();

        return services;
    }

    /// <summary>
    /// Validates the client configuration; throw if not valid.
    /// </summary>
    /// <param name="clientConfiguration">The <see cref="ClientConfiguration"/>.</param>
    /// <param name="clientName">The name of the client.</param>
    /// <returns>The valid <see cref="ClientConfiguration"/>.</returns>
    /// <exception cref="ConfigurationErrorsException">The exception that is thrown when a configuration error has occurred.</exception>
    private static ClientConfiguration ValidateOrThrow(
        this ClientConfiguration? clientConfiguration,
        string clientName)
    {
        return Validate(clientConfiguration)
            ? clientConfiguration!
            : throw new ConfigurationErrorsException($"Configuration error for 'Clients.{clientName}'.");
    }

    /// <summary>
    /// Validates the client configuration.
    /// </summary>
    /// <param name="clientConfiguration">The <see cref="ClientConfiguration"/>.</param>
    /// <returns>true if the <see cref="ClientConfiguration"/> is valid; otherwise, false.</returns>
    private static bool Validate(
        ClientConfiguration? clientConfiguration)
    {
        if (clientConfiguration?.BaseUri is null) return false;
        if (string.IsNullOrWhiteSpace(clientConfiguration?.CredentialProviderName)) return false;
        if (string.IsNullOrWhiteSpace(clientConfiguration?.Scope)) return false;

        return true;
    }
}
