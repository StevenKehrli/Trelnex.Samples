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
    /// <param name="userId">The specified user id.</param>
    /// <param name="request">The <see cref="CreateMessageRequest"/>.</param>
    /// <returns>The new <see cref="MessageModel"/>.</returns>
    Task<MessageModel> CreateMessage(
        Guid userId,
        CreateMessageRequest request);

    /// <summary>
    /// Deletes the specified message.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <param name="messageId">The specified message id.</param>
    Task<DeleteMessageResponse> DeleteMessage(
        Guid userId,
        Guid messageId);

    /// <summary>
    /// Gets the specified message, if it exists.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <returns>The <see cref="MessageModel"/>, if it exists.</returns>
    Task<MessageModel> GetMessage(
        Guid userId,
        Guid messageId);

    /// <summary>
    /// Gets the messages from the specified user.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The array of <see cref="MessageModel"/>.</returns>
    Task<MessageModel[]> GetMessages(
        Guid userId);

    /// <summary>
    /// Updates the specified message.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <param name="request">The <see cref="UpdateMessageRequest"/>.</param>
    /// <returns>The updated <see cref="MessageModel"/>.</returns>
    Task<MessageModel> UpdateMessage(
        Guid userId,
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
    IAccessTokenProvider tokenProvider)
    : BaseClient(httpClient, tokenProvider), IMessagesClient
{
    /// <summary>
    /// Creates the specified message.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <param name="request">The <see cref="CreateMessageRequest"/>.</param>
    /// <returns>The new <see cref="MessageModel"/>.</returns>
    public async Task<MessageModel> CreateMessage(
        Guid userId,
        CreateMessageRequest request)
    {
        return await Post<CreateMessageRequest, MessageModel>(
            uri: BaseAddress.AppendPath($"/{userId}/messages"),
            content: request);
    }

    /// <summary>
    /// Deletes the specified message.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <param name="messageId">The specified message id.</param>
    public async Task<DeleteMessageResponse> DeleteMessage(
        Guid userId,
        Guid messageId)
    {
        return await Delete<DeleteMessageResponse>(
            uri: BaseAddress.AppendPath($"useres/{userId}/messages/{messageId}"));
    }

    /// <summary>
    /// Gets the specified message, if it exists.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <returns>The <see cref="MessageModel"/>, if it exists.</returns>
    public async Task<MessageModel> GetMessage(
        Guid userId,
        Guid messageId)
    {
        return await Get<MessageModel>(
            uri: BaseAddress.AppendPath($"useres/{userId}/messages/{messageId}"));
    }

    /// <summary>
    /// Gets the message from the specified user.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The array of <see cref="MessageModel"/>.</returns>
    public async Task<MessageModel[]> GetMessages(
        Guid userId)
    {
        return await Get<MessageModel[]>(
            uri: BaseAddress.AppendPath($"useres/{userId}"));
    }

    /// <summary>
    /// Updates the specified message
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <param name="request">The <see cref="UpdateMessageRequest"/>.</param>
    /// <returns>The updated <see cref="MessageModel"/>.</returns>
    public async Task<MessageModel> UpdateMessage(
        Guid userId,
        Guid messageId,
        UpdateMessageRequest request)
    {
        return await Put<UpdateMessageRequest, MessageModel>(
            uri: BaseAddress.AppendPath($"useres/{userId}/messages/{messageId}"),
            content: request);
    }
}
