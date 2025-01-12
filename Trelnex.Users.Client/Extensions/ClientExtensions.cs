using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trelnex.Core.Client.Identity;

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
    /// <param name="bootstrapLogger">The <see cref="ILogger"/> to write the CommandProvider bootstrap logs.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithUsersClient(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger bootstrapLogger)
    {
        var clientOptions = configuration
            .GetSection("Clients")
            .GetSection(UsersClient.Name)
            .Get<UsersClientConfiguration>()
            .ValidateOrThrow();

        services.AddHttpClient(UsersClient.Name);

        // get the token credential and request context
        var tokenCredential = CredentialFactory.Instance.Get(UsersClient.Name);
        var tokenRequestContext = new TokenRequestContext([clientOptions.Scope]);

        var getAuthorizationHeader = () => tokenCredential.GetAuthorizationHeader(tokenRequestContext);

        // initialize
        getAuthorizationHeader();

        services.AddScoped<IUsersClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            return new UsersClient(httpClientFactory, getAuthorizationHeader, clientOptions.BaseUri);

        });

        return services;
    }

    /// <summary>
    /// Validates the users client configuration; throw if not valid.
    /// </summary>
    /// <param name="usersClientConfiguration">The <see cref="UsersClientConfiguration"/>.</param>
    /// <returns>The valid <see cref="UsersClientConfiguration"/>.</returns>
    /// <exception cref="ConfigurationErrorsException">The exception that is thrown when a configuration error has occurred.</exception>
    private static UsersClientConfiguration ValidateOrThrow(
        this UsersClientConfiguration? usersClientConfiguration)
    {
        return Validate(usersClientConfiguration)
            ? usersClientConfiguration!
            : throw new InvalidOperationException($"Configuration error for 'Clients.{UsersClient.Name}'.");
    }

    /// <summary>
    /// Validates the users client configuration.
    /// </summary>
    /// <param name="usersClientConfiguration">The <see cref="UsersClientConfiguration"/>.</param>
    /// <returns>true if the <see cref="UsersClientConfiguration"/> is valid; otherwise, false.</returns>
    private static bool Validate(
        UsersClientConfiguration? usersClientConfiguration)
    {
        if (usersClientConfiguration?.BaseUri is null) return false;
        if (string.IsNullOrWhiteSpace(usersClientConfiguration?.Scope)) return false;

        return true;
    }

    /// <summary>
    /// Represents the configuration properties for the users client.
    /// </summary>
    /// <param name="BaseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
    /// <param name="Scope">The required scope for the <see cref="AccessToken"/>.</param>
    private record UsersClientConfiguration(
        Uri BaseUri,
        string Scope);
}
