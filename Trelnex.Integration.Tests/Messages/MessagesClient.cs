using Trelnex.Core.Data;
using Trelnex.Messages.Api.Endpoints;
using Trelnex.Messages.Api.Items;
using Trelnex.Messages.Client;
using Trelnex.Users.Client;

namespace Trelnex.Integration.Tests.Messages;

internal class MessagesClient(
    IUsersClient usersClient,
    IDataProvider<IMessageItem> messageProvider) : IMessagesClient
{
    public async Task<MessageModel> CreateMessage(
        Guid userId,
        CreateMessageRequest request)
    {
        // format the request arguments
        var parameters = new CreateMessageEndpoint.RequestParameters()
        {
            UserId = userId,
            Request = request
        };

        // invoke
        return await CreateMessageEndpoint.HandleRequest(
            usersClient: usersClient,
            messageProvider: messageProvider,
            parameters: parameters);
    }

    public async Task<DeleteMessageResponse> DeleteMessage(
        Guid userId,
        Guid messageId)
    {
        // format the request arguments
        var parameters = new DeleteMessageEndpoint.RequestParameters()
        {
            UserId = userId,
            MessageId = messageId
        };

        // invoke
        return await DeleteMessageEndpoint.HandleRequest(
            usersClient: usersClient,
            messageProvider: messageProvider,
            parameters: parameters);
    }

    public async Task<MessageModel> GetMessage(
        Guid userId,
        Guid messageId)
    {
        // format the request arguments
        var parameters = new GetMessageEndpoint.RequestParameters()
        {
            UserId = userId,
            MessageId = messageId
        };

        // invoke
        return await GetMessageEndpoint.HandleRequest(
            usersClient: usersClient,
            messageProvider: messageProvider,
            parameters: parameters);
    }

    public async Task<MessageModel[]> GetMessages(
        Guid userId)
    {
        // format the request arguments
        var parameters = new GetMessagesEndpoint.RequestParameters()
        {
            UserId = userId
        };

        // invoke
        return await GetMessagesEndpoint.HandleRequest(
            usersClient: usersClient,
            messageProvider: messageProvider,
            parameters: parameters);
    }

    public async Task<MessageModel> UpdateMessage(
        Guid userId,
        Guid messageId,
        UpdateMessageRequest request)
    {
        // format the request arguments
        var parameters = new UpdateMessageEndpoint.RequestParameters()
        {
            UserId = userId,
            MessageId = messageId,
            Request = request
        };

        // invoke
        return await UpdateMessageEndpoint.HandleRequest(
            usersClient: usersClient,
            messageProvider: messageProvider,
            parameters: parameters);
    }
}
