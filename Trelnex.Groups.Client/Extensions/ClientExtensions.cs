using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trelnex.Core.Client.Identity;

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
    /// <param name="bootstrapLogger">The <see cref="ILogger"/> to write the CommandProvider bootstrap logs.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithGroupsClient(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger bootstrapLogger)
    {
        var clientOptions = configuration
            .GetSection("Clients")
            .GetSection(GroupsClient.Name)
            .Get<GroupsClientConfiguration>()
            .ValidateOrThrow();

        services.AddHttpClient(GroupsClient.Name);

        // get the token credential and initialize
        var tokenCredential = CredentialFactory.Get(bootstrapLogger, GroupsClient.Name);

        var tokenRequestContext = new TokenRequestContext([clientOptions.Scope]);
        tokenCredential.GetToken(tokenRequestContext, default);

        services.AddScoped<IGroupsClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            return new GroupsClient(httpClientFactory, tokenCredential, tokenRequestContext, clientOptions.BaseUri);

        });

        return services;
    }

    /// <summary>
    /// Validates the groups client configuration; throw if not valid.
    /// </summary>
    /// <param name="groupsClientConfiguration">The <see cref="GroupsClientConfiguration"/>.</param>
    /// <returns>The valid <see cref="GroupsClientConfiguration"/>.</returns>
    /// <exception cref="ConfigurationErrorsException">The exception that is thrown when a configuration error has occurred.</exception>
    private static GroupsClientConfiguration ValidateOrThrow(
        this GroupsClientConfiguration? groupsClientConfiguration)
    {
        return Validate(groupsClientConfiguration)
            ? groupsClientConfiguration!
            : throw new InvalidOperationException($"Configuration error for 'Clients.{GroupsClient.Name}'.");
    }

    /// <summary>
    /// Validates the groups client configuration.
    /// </summary>
    /// <param name="groupsClientConfiguration">The <see cref="GroupsClientConfiguration"/>.</param>
    /// <returns>true if the <see cref="GroupsClientConfiguration"/> is valid; otherwise, false.</returns>
    private static bool Validate(
        GroupsClientConfiguration? groupsClientConfiguration)
    {
        if (groupsClientConfiguration?.BaseUri is null) return false;
        if (string.IsNullOrWhiteSpace(groupsClientConfiguration?.Scope)) return false;

        return true;
    }

    /// <summary>
    /// Represents the configuration properties for the groups client.
    /// </summary>
    /// <param name="BaseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
    /// <param name="Scope">The required scope for the <see cref="AccessToken"/>.</param>
    private record GroupsClientConfiguration(
        Uri BaseUri,
        string Scope);
}
