using Trelnex.Core.Client;
using Trelnex.Core.Identity;

namespace Trelnex.Messages.Client;

/// <summary>
/// Defines the contract for a Messages client.
/// </summary>
public interface IMessagesClient
{
    /// <summary>
    /// Creates the specified message.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="request">The <see cref="CreateMessageRequest"/>.</param>
    /// <returns>The new <see cref="MessageModel"/>.</returns>
    Task<MessageModel> CreateMessage(
        Guid mailboxId,
        CreateMessageRequest request);

    /// <summary>
    /// Deletes the specified message.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="messageId">The specified message id.</param>
    Task<DeleteMessageResponse> DeleteMessage(
        Guid mailboxId,
        Guid messageId);

    /// <summary>
    /// Gets the specified message, if it exists.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <returns>The <see cref="MessageModel"/>, if it exists.</returns>
    Task<MessageModel> GetMessage(
        Guid mailboxId,
        Guid messageId);

    /// <summary>
    /// Gets the messages from the specified mailbox.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <returns>The array of <see cref="MessageModel"/>.</returns>
    Task<MessageModel[]> GetMessages(
        Guid mailboxId);

    /// <summary>
    /// Updates the specified message.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <param name="request">The <see cref="UpdateMessageRequest"/>.</param>
    /// <returns>The updated <see cref="MessageModel"/>.</returns>
    Task<MessageModel> UpdateMessage(
        Guid mailboxId,
        Guid messageId,
        UpdateMessageRequest request);
}

/// <summary>
/// Initializes a new instance of the <see cref="MessagesClient"/>.
/// </summary>
/// <param name="httpClient">The specified <see cref="HttpClient"/> instance.</param>
/// <param name="tokenProvider">The specified <see cref="IAccessTokenProvider"/> to get the access token.</param>
internal class MessagesClient(
    HttpClient httpClient,
    IAccessTokenProvider<MessagesClient> tokenProvider)
    : BaseClient(httpClient), IMessagesClient
{
    /// <summary>
    /// Creates the specified message.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="request">The <see cref="CreateMessageRequest"/>.</param>
    /// <returns>The new <see cref="MessageModel"/>.</returns>
    public async Task<MessageModel> CreateMessage(
        Guid mailboxId,
        CreateMessageRequest request)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Post<CreateMessageRequest, MessageModel>(
            uri: BaseAddress.AppendPath($"/{request.MailboxId}/messages"),
            content: request,
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Deletes the specified message.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="messageId">The specified message id.</param>
    public async Task<DeleteMessageResponse> DeleteMessage(
        Guid mailboxId,
        Guid messageId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Delete<DeleteMessageResponse>(
            uri: BaseAddress.AppendPath($"mailboxes/{mailboxId}/messages/{messageId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Gets the specified message, if it exists.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <returns>The <see cref="MessageModel"/>, if it exists.</returns>
    public async Task<MessageModel> GetMessage(
        Guid mailboxId,
        Guid messageId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Get<MessageModel>(
            uri: BaseAddress.AppendPath($"mailboxes/{mailboxId}/messages/{messageId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Gets the message from the specified mailbox.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <returns>The array of <see cref="MessageModel"/>.</returns>
    public async Task<MessageModel[]> GetMessages(
        Guid mailboxId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Get<MessageModel[]>(
            uri: BaseAddress.AppendPath($"mailboxes/{mailboxId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Updates the specified message
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <param name="request">The <see cref="UpdateMessageRequest"/>.</param>
    /// <returns>The updated <see cref="MessageModel"/>.</returns>
    public async Task<MessageModel> UpdateMessage(
        Guid mailboxId,
        Guid messageId,
        UpdateMessageRequest request)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Put<UpdateMessageRequest, MessageModel>(
            uri: BaseAddress.AppendPath($"mailboxes/{mailboxId}/messages/{messageId}"),
            content: request,
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }
}
