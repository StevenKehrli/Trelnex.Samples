using Azure.Core;
using Trelnex.Core.Client;

namespace Trelnex.Mailboxes.Client;

/// <summary>
/// Defines the contract for a Mailboxes client.
/// </summary>
public interface IMailboxesClient
{
    /// <summary>
    /// Get the mailbox for the specified group. Creates the mailbox if it does not exist.
    /// </summary>
    /// <param name="groupId">The specified group id.</param>
    /// <returns>The <see cref="MailboxModel"/>.</returns>
    Task<MailboxModel> GetGroupMailbox(
        Guid groupId);

    /// <summary>
    /// Gets the specified mailbox, if it exists.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <returns>The <see cref="MailboxModel"/>, if it exists.</returns>
    Task<MailboxModel> GetMailbox(
        Guid mailboxId);

    /// <summary>
    /// Get the mailbox for the specified user. Creates the mailbox if it does not exist.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The <see cref="MailboxModel"/>.</returns>
    Task<MailboxModel> GetUserMailbox(
        Guid userId);
}

/// <summary>
/// Initializes a new instance of the <see cref="MailboxesClient"/>.
/// </summary>
/// <param name="httpClientFactory">The specified <see cref="IHttpClientFactory"/> to create and configure an <see cref="HttpClient"/> instance.</param>
/// <param name="tokenCredential">The specified <see cref="TokenCredential"/> to get the <see cref="AccessToken"/> for the specified set of scopes.</param>
/// <param name="tokenRequestContext">The <see cref="TokenRequestContext"/> with authentication information.</param>
/// <param name="baseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
public class MailboxesClient(
    IHttpClientFactory httpClientFactory,
    TokenCredential tokenCredential,
    TokenRequestContext tokenRequestContext,
    Uri baseUri)
    : BaseClient(httpClientFactory), IMailboxesClient
{
    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    public static string Name => "Mailboxes";

    /// <summary>
    /// Get the mailbox for the specified group. Creates the mailbox if it does not exist.
    /// </summary>
    /// <param name="groupId">The specified group id.</param>
    /// <returns>The <see cref="MailboxModel"/>.</returns>
    public async Task<MailboxModel> GetGroupMailbox(
        Guid groupId)
    {
        return await Get<MailboxModel>(
            uri: baseUri.AppendPath($"/groups/{groupId}/mailbox"),
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
    }

    /// <summary>
    /// Gets the specified mailbox, if it exists.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <returns>The <see cref="MailboxModel"/>, if it exists.</returns>
    public async Task<MailboxModel> GetMailbox(
        Guid mailboxId)
    {
        return await Get<MailboxModel>(
            uri: baseUri.AppendPath($"/mailboxes/{mailboxId}"),
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
    }

    /// <summary>
    /// Get the mailbox for the specified user. Creates the mailbox if it does not exist.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The <see cref="MailboxModel"/>.</returns>
    public async Task<MailboxModel> GetUserMailbox(
        Guid userId)
    {
        return await Get<MailboxModel>(
            uri: baseUri.AppendPath($"/users/{userId}/mailbox"),
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
    }

    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    protected override string GetName() => Name;
}
