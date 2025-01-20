namespace Trelnex.Client.Common;

/// <summary>
/// Represents the configuration properties for a client.
/// </summary>
/// <param name="CredentialProviderName">The name of the <see cref="ICredentialProvider"/> to get the <see cref="AccessToken"/>.</param>
/// <param name="Scope">The required scope for the <see cref="AccessToken"/>.</param>
/// <param name="BaseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
public record ClientConfiguration(
    string CredentialProviderName,
    string Scope,
    Uri BaseUri);
