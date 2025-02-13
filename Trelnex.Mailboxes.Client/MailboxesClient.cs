using Trelnex.Core.Client;
using Trelnex.Core.Identity;

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
/// <param name="httpClient">The specified <see cref="HttpClient"/> instance.</param>
/// <param name="tokenProvider">The specified <see cref="IAccessTokenProvider"/> to get the access token.</param>
internal class MailboxesClient(
    HttpClient httpClient,
    IAccessTokenProvider<MailboxesClient> tokenProvider)
    : BaseClient(httpClient), IMailboxesClient
{
    /// <summary>
    /// Get the mailbox for the specified group. Creates the mailbox if it does not exist.
    /// </summary>
    /// <param name="groupId">The specified group id.</param>
    /// <returns>The <see cref="MailboxModel"/>.</returns>
    public async Task<MailboxModel> GetGroupMailbox(
        Guid groupId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Get<MailboxModel>(
            uri: BaseAddress.AppendPath($"/groups/{groupId}/mailbox"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Gets the specified mailbox, if it exists.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <returns>The <see cref="MailboxModel"/>, if it exists.</returns>
    public async Task<MailboxModel> GetMailbox(
        Guid mailboxId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Get<MailboxModel>(
            uri: BaseAddress.AppendPath($"/mailboxes/{mailboxId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Get the mailbox for the specified user. Creates the mailbox if it does not exist.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The <see cref="MailboxModel"/>.</returns>
    public async Task<MailboxModel> GetUserMailbox(
        Guid userId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Get<MailboxModel>(
            uri: BaseAddress.AppendPath($"/users/{userId}/mailbox"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }
}
