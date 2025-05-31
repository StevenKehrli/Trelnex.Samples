using System.Linq.Expressions;
using Trelnex.Integration.Tests.InMemory;
using Trelnex.Integration.Tests.Users;
using Trelnex.Messages.Api.Items;
using Trelnex.Messages.Client;

namespace Trelnex.Integration.Tests.Messages;

public class MessagesContext
{
    private IMessagesClient _messagesClient = null!;

    private readonly Dictionary<string, MessageModel> _messageModels = [];

    public MessagesContext(
        UsersContext usersContext)
    {
        // create the messages command providers
        var inMemoryCommandProviders = InMemoryCommandProviders.Create(
            options => options.AddMessagesCommandProviders());

        // get the messages command provider
        var messageProvider = inMemoryCommandProviders.Get<IMessageItem>();

        // create the messages client
        _messagesClient = new MessagesClient(
            usersClient: usersContext.Client,
            messageProvider: messageProvider);
    }

    public IMessagesClient Client => _messagesClient;

    public void Add(
        MessageModel messageModel)
    {
        var key = $"{messageModel.UserId}:{messageModel.MessageId}";

        _messageModels[key] = messageModel;
    }

    public bool Exists(
        Expression<Func<MessageModel, bool>> predicate)
    {
        return _messageModels.Values.AsQueryable().Any(predicate);

    }

    public MessageModel Single(
        Expression<Func<MessageModel, bool>> predicate)
    {
        return _messageModels.Values.AsQueryable().Single(predicate);
    }
}
