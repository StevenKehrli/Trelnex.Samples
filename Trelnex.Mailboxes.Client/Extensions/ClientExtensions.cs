using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trelnex.Core.Client.Identity;

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
    /// <param name="bootstrapLogger">The <see cref="ILogger"/> to write the CommandProvider bootstrap logs.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection WithMailboxesClient(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger bootstrapLogger)
    {
        var clientOptions = configuration
            .GetSection("Clients")
            .GetSection(MailboxesClient.Name)
            .Get<MailboxesClientConfiguration>()
            .ValidateOrThrow();

        services.AddHttpClient(MailboxesClient.Name);

        // get the token credential and initialize
        var tokenCredential = CredentialFactory.Get(bootstrapLogger, MailboxesClient.Name);

        var tokenRequestContext = new TokenRequestContext([clientOptions.Scope]);
        tokenCredential.GetToken(tokenRequestContext, default);

        services.AddScoped<IMailboxesClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            return new MailboxesClient(httpClientFactory, tokenCredential, tokenRequestContext, clientOptions.BaseUri);

        });

        return services;
    }

    /// <summary>
    /// Validates the mailboxes client configuration; throw if not valid.
    /// </summary>
    /// <param name="mailboxesClientConfiguration">The <see cref="MailboxesClientConfiguration"/>.</param>
    /// <returns>The valid <see cref="MailboxesClientConfiguration"/>.</returns>
    /// <exception cref="ConfigurationErrorsException">The exception that is thrown when a configuration error has occurred.</exception>
    private static MailboxesClientConfiguration ValidateOrThrow(
        this MailboxesClientConfiguration? mailboxesClientConfiguration)
    {
        return Validate(mailboxesClientConfiguration)
            ? mailboxesClientConfiguration!
            : throw new InvalidOperationException($"Configuration error for 'Clients.{MailboxesClient.Name}'.");
    }

    /// <summary>
    /// Validates the mailboxes client configuration.
    /// </summary>
    /// <param name="mailboxesClientConfiguration">The <see cref="MailboxesClientConfiguration"/>.</param>
    /// <returns>true if the <see cref="MailboxesClientConfiguration"/> is valid; otherwise, false.</returns>
    private static bool Validate(
        MailboxesClientConfiguration? mailboxesClientConfiguration)
    {
        if (mailboxesClientConfiguration?.BaseUri is null) return false;
        if (string.IsNullOrWhiteSpace(mailboxesClientConfiguration?.Scope)) return false;

        return true;
    }

    /// <summary>
    /// Represents the configuration properties for the mailboxes client.
    /// </summary>
    /// <param name="BaseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
    /// <param name="Scope">The required scope for the <see cref="AccessToken"/>.</param>
    private record MailboxesClientConfiguration(
        Uri BaseUri,
        string Scope);
}
