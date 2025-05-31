using System.Net;
using Reqnroll;
using Trelnex.Core;
using Trelnex.Integration.Tests.Users;
using Trelnex.Messages.Client;

namespace Trelnex.Integration.Tests.Messages;

[Binding]
public class MessagesStepDefinitions
{
    private UsersContext _usersContext = null!;
    private MessagesContext _messagesContext = null!;

    [BeforeScenario("messages")]
    public void BeforeScenario(
        UsersContext usersContext,
        MessagesContext messagesContext)
    {
        _usersContext = usersContext;
        _messagesContext = messagesContext;
    }

    [AfterScenario("messages")]
    public void AfterScenario()
    {
        _usersContext = null!;
        _messagesContext = null!;
    }

    [Given(@"Message for User (.*) with contents '(.*)' exists")]
    [When(@"Message for User (.*) with contents '(.*)' is created")]
    public void UserMessageCreated(
        string userName,
        string contents)
    {
        if (_messagesContext.Exists(mm => mm.Contents == contents))
        {
            return;
        }

        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // create the request
        var request = new CreateMessageRequest()
        {
            Contents = contents
        };

        // create the message
        var messageModel = _messagesContext.Client.CreateMessage(
            userId: userModel.Id,
            request: request).Result;

        _messagesContext.Add(messageModel);
    }

    [When(@"Message for User (.*) with contents '(.*)' is updated to contents '(.*)'")]
    public void UserMessageUpdated(
        string userName,
        string contents,
        string updatedContents)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.UserId == userModel.Id && mm.Contents == contents);

        // create the request
        var request = new UpdateMessageRequest()
        {
            Contents = updatedContents
        };

        // update the message
        var messageModel = _messagesContext.Client.UpdateMessage(
            userId: messageModelFromContext.UserId,
            messageId: messageModelFromContext.MessageId,
            request: request).Result;

        _messagesContext.Add(messageModel);
    }

    [When(@"Message for User (.*) with contents '(.*)' is deleted")]
    public void UserMessageDeleted(
        string userName,
        string contents)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.UserId == userModel.Id && mm.Contents == contents);

        // delete the message
        var messageModel = _messagesContext.Client.DeleteMessage(
            userId: messageModelFromContext.UserId,
            messageId: messageModelFromContext.MessageId).Result;
    }

    [Then(@"Message for User (.*) with contents '(.*)' is valid")]
    public void UserMessageIsValid(
        string userName,
        string contents)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.UserId == userModel.Id && mm.Contents == contents);

        // get the message
        var messageModel = _messagesContext.Client.GetMessage(
            userId: messageModelFromContext.UserId,
            messageId: messageModelFromContext.MessageId).Result;

        Assert.Multiple(() =>
        {
            Assert.That(messageModel.UserId, Is.EqualTo(messageModelFromContext.UserId));
            Assert.That(messageModel.MessageId, Is.EqualTo(messageModelFromContext.MessageId));
            Assert.That(messageModel.Contents, Is.EqualTo(contents));
        });
    }

    [Then(@"Messages for User (.*) are valid")]
    public void UserMessagesAreValid(
        string userName,
        Table table)
    {
        var expectedTable = table.Rows.Select(row => row["Contents"]).ToArray();

        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the messages
        var messageModels = _messagesContext.Client.GetMessages(
            userId: userModel.Id).Result;

        var actualTable = messageModels.Select(mm => mm.Contents).ToArray();

        Assert.That(actualTable, Is.EqualTo(expectedTable));
    }

    [Then(@"Message for User (.*) with contents '(.*)' is not found")]
    public void UserMessageIsNotFound(
        string userName,
        string contents)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.UserId == userModel.Id && mm.Contents == contents);

        // get the message
        var ex = Assert.Throws<HttpStatusCodeException>(() =>
            _messagesContext.Client
                .GetMessage(
                    userId: messageModelFromContext.UserId,
                    messageId: messageModelFromContext.MessageId)
                .GetAwaiter()
                .GetResult());

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Then(@"UpdateMessage for User (.*) with bad messageId throws NotFound")]
    public void UpdateMessageBadMessageId(
        string userName)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        var messageId = Guid.NewGuid();

        // create the request
        var request = new UpdateMessageRequest()
        {
            Contents = string.Empty
        };

        // update the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.UpdateMessage(
                userId: userModel.Id,
                messageId: messageId,
                request: request));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Then(@"DeleteMessage for User (.*) with bad messageId throws NotFound")]
    public void DeleteMessageBadMessageId(
        string userName)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        var messageId = Guid.NewGuid();

        // delete the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.DeleteMessage(
                userId: userModel.Id,
                messageId: messageId));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
