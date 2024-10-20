using Azure.Core;
using Trelnex.Core.Client;

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
/// <param name="httpClientFactory">The specified <see cref="IHttpClientFactory"/> to create and configure an <see cref="HttpClient"/> instance.</param>
/// <param name="tokenCredential">The specified <see cref="TokenCredential"/> to get the <see cref="AccessToken"/> for the specified set of scopes.</param>
/// <param name="tokenRequestContext">The <see cref="TokenRequestContext"/> with authentication information.</param>
/// <param name="baseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
public class MessagesClient(
    IHttpClientFactory httpClientFactory,
    TokenCredential tokenCredential,
    TokenRequestContext tokenRequestContext,
    Uri baseUri)
    : BaseClient(httpClientFactory), IMessagesClient
{
    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    public static string Name => "Messages";

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
        return await Post<CreateMessageRequest, MessageModel>(
            uri: baseUri.AppendPath($"/{request.MailboxId}/messages"),
            content: request,
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
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
        return await Delete<DeleteMessageResponse>(
            uri: baseUri.AppendPath($"mailboxes/{mailboxId}/messages/{messageId}"),
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
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
        return await Get<MessageModel>(
            uri: baseUri.AppendPath($"mailboxes/{mailboxId}/messages/{messageId}"),
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
    }

    /// <summary>
    /// Gets the message from the specified mailbox.
    /// </summary>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <returns>The array of <see cref="MessageModel"/>.</returns>
    public async Task<MessageModel[]> GetMessages(
        Guid mailboxId)
    {
        return await Get<MessageModel[]>(
            uri: baseUri.AppendPath($"mailboxes/{mailboxId}"),
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
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
        return await Put<UpdateMessageRequest, MessageModel>(
            uri: baseUri.AppendPath($"mailboxes/{mailboxId}/messages/{messageId}"),
            content: request,
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
    }

    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    protected override string GetName() => Name;
}
