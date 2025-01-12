using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trelnex.Core.Client.Identity;

namespace Trelnex.Messages.Client;

/// <summary>
/// Extension methods to add the <see cref="IMessagesClient"/> to the <see cref="IServiceCollection"/>.
/// </summary>
public static class ClientExtensions
{
    /// <summary>
    /// Add the <see cref="IMessagesClient"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
    /// <param name="bootstrapLogger">The <see cref="ILogger"/> to write the CommandProvider bootstrap logs.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithMessagesClient(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger bootstrapLogger)
    {
        var clientOptions = configuration
            .GetSection("Clients")
            .GetSection(MessagesClient.Name)
            .Get<MessagesClientConfiguration>()
            .ValidateOrThrow();

        services.AddHttpClient(MessagesClient.Name);

        // get the token credential and request context
        var tokenCredential = CredentialFactory.Instance.Get(MessagesClient.Name);
        var tokenRequestContext = new TokenRequestContext([clientOptions.Scope]);

        var getAuthorizationHeader = () => tokenCredential.GetAuthorizationHeader(tokenRequestContext);

        // initialize
        getAuthorizationHeader();

        services.AddScoped<IMessagesClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            return new MessagesClient(httpClientFactory, getAuthorizationHeader, clientOptions.BaseUri);

        });

        return services;
    }

    /// <summary>
    /// Validates the messages client configuration; throw if not valid.
    /// </summary>
    /// <param name="messagesClientConfiguration">The <see cref="MessagesClientConfiguration"/>.</param>
    /// <returns>The valid <see cref="MessagesClientConfiguration"/>.</returns>
    /// <exception cref="ConfigurationErrorsException">The exception that is thrown when a configuration error has occurred.</exception>
    private static MessagesClientConfiguration ValidateOrThrow(
        this MessagesClientConfiguration? messagesClientConfiguration)
    {
        return Validate(messagesClientConfiguration)
            ? messagesClientConfiguration!
            : throw new InvalidOperationException($"Configuration error for 'Clients.{MessagesClient.Name}'.");
    }

    /// <summary>
    /// Validates the messages client configuration.
    /// </summary>
    /// <param name="messagesClientConfiguration">The <see cref="MessagesClientConfiguration"/>.</param>
    /// <returns>true if the <see cref="MessagesClientConfiguration"/> is valid; otherwise, false.</returns>
    private static bool Validate(
        MessagesClientConfiguration? messagesClientConfiguration)
    {
        if (messagesClientConfiguration?.BaseUri is null) return false;
        if (string.IsNullOrWhiteSpace(messagesClientConfiguration?.Scope)) return false;

        return true;
    }

    /// <summary>
    /// Represents the configuration properties for the messages client.
    /// </summary>
    /// <param name="BaseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
    /// <param name="Scope">The required scope for the <see cref="AccessToken"/>.</param>
    private record MessagesClientConfiguration(
        Uri BaseUri,
        string Scope);
}
