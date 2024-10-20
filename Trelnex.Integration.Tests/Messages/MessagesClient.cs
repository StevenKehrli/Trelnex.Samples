using Trelnex.Core.Data;
using Trelnex.Mailboxes.Client;
using Trelnex.Messages.Api.Endpoints;
using Trelnex.Messages.Api.Objects;
using Trelnex.Messages.Client;

namespace Trelnex.Integration.Tests.Messages;

internal class MessagesClient(
    IMailboxesClient mailboxesClient,
    ICommandProvider<IMessage> messageProvider) : IMessagesClient
{
    public async Task<MessageModel> CreateMessage(
        Guid mailboxId,
        CreateMessageRequest request)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new CreateMessageEndpoint.RequestParameters()
        {
            MailboxId = mailboxId,
            Request = request
        };

        // invoke
        return await CreateMessageEndpoint.HandleRequest(
            mailboxesClient: mailboxesClient,
            messageProvider: messageProvider,
            requestContext: requestContext,
            parameters: parameters);
    }

    public async Task<DeleteMessageResponse> DeleteMessage(
        Guid mailboxId,
        Guid messageId)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new DeleteMessageEndpoint.RequestParameters()
        {
            MailboxId = mailboxId,
            MessageId = messageId
        };

        // invoke
        return await DeleteMessageEndpoint.HandleRequest(
            mailboxesClient: mailboxesClient,
            messageProvider: messageProvider,
            requestContext: requestContext,
            parameters: parameters);
    }

    public async Task<MessageModel> GetMessage(
        Guid mailboxId,
        Guid messageId)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new GetMessageEndpoint.RequestParameters()
        {
            MailboxId = mailboxId,
            MessageId = messageId
        };

        // invoke
        return await GetMessageEndpoint.HandleRequest(
            mailboxesClient: mailboxesClient,
            messageProvider: messageProvider,
            parameters: parameters);
    }

    public async Task<MessageModel[]> GetMessages(
        Guid mailboxId)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new GetMessagesEndpoint.RequestParameters()
        {
            MailboxId = mailboxId
        };

        // invoke
        return await GetMessagesEndpoint.HandleRequest(
            mailboxesClient: mailboxesClient,
            messageProvider: messageProvider,
            parameters: parameters);
    }

    public async Task<MessageModel> UpdateMessage(
        Guid mailboxId,
        Guid messageId,
        UpdateMessageRequest request)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new UpdateMessageEndpoint.RequestParameters()
        {
            MailboxId = mailboxId,
            MessageId = messageId,
            Request = request
        };

        // invoke
        return await UpdateMessageEndpoint.HandleRequest(
            mailboxesClient: mailboxesClient,
            messageProvider: messageProvider,
            requestContext: requestContext,
            parameters: parameters);
    }
}
