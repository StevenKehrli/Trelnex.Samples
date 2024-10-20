using System.Net;
using TechTalk.SpecFlow;
using Trelnex.Core;
using Trelnex.Integration.Tests.Groups;
using Trelnex.Integration.Tests.Mailboxes;
using Trelnex.Integration.Tests.Users;
using Trelnex.Mailboxes.Client;
using Trelnex.Messages.Client;

namespace Trelnex.Integration.Tests.Messages;

[Binding]
public class MessagesStepDefinitions
{
    private UsersContext _usersContext = null!;
    private GroupsContext _groupsContext = null!;
    private MailboxesContext _mailboxesContext = null!;
    private MessagesContext _messagesContext = null!;

    [BeforeScenario("messages")]
    public void BeforeScenario(
        UsersContext usersContext,
        GroupsContext groupsContext,
        MailboxesContext mailboxesContext,
        MessagesContext messagesContext)
    {
        _usersContext = usersContext;
        _groupsContext = groupsContext;
        _mailboxesContext = mailboxesContext;
        _messagesContext = messagesContext;
    }

    [AfterScenario("messages")]
    public void AfterScenario()
    {
        _usersContext = null!;
        _groupsContext = null!;
        _mailboxesContext = null!;
        _messagesContext = null!;
    }

    [Given(@"Message with contents '(.*)' in UserMailbox for (.*) exists")]
    [When(@"Message with contents '(.*)' in UserMailbox for (.*) is created")]
    public async void UserMessageCreated(
        string contents,
        string userName)
    {
        if (_messagesContext.Exists(mm => mm.Contents == contents))
        {
            return;
        }

        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == userModel.Id && mm.OwnerType == MailboxOwnerType.User);

        // create the request
        var request = new CreateMessageRequest()
        {
            MailboxId = mailboxModel.Id,
            Contents = contents
        };

        // create the message
        var messageModel = await _messagesContext.Client.CreateMessage(
            mailboxId: mailboxModel.Id,
            request: request);

        _messagesContext.Add(messageModel);
    }

    [When(@"Message with contents '(.*)' in UserMailbox for (.*) is updated to contents '(.*)'")]
    public async void UserMessageUpdated(
        string contents,
        string userName,
        string updatedContents)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == userModel.Id && mm.OwnerType == MailboxOwnerType.User);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // create the request
        var request = new UpdateMessageRequest()
        {
            MailboxId = mailboxModel.Id,
            MessageId = messageModelFromContext.MessageId,
            Contents = updatedContents
        };

        // update the message
        var messageModel = await _messagesContext.Client.UpdateMessage(
            mailboxId: messageModelFromContext.MailboxId,
            messageId: messageModelFromContext.MessageId,
            request: request);

        _messagesContext.Add(messageModel);
    }

    [When(@"Message with contents '(.*)' in UserMailbox for (.*) is deleted")]
    public async void UserMessageDeleted(
        string contents,
        string userName)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == userModel.Id && mm.OwnerType == MailboxOwnerType.User);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // delete the message
        var messageModel = await _messagesContext.Client.DeleteMessage(
            mailboxId: messageModelFromContext.MailboxId,
            messageId: messageModelFromContext.MessageId);
    }

    [Then(@"Message with contents '(.*)' in UserMailbox for (.*) is valid")]
    public async void UserMessageIsValid(
        string contents,
        string userName)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == userModel.Id && mm.OwnerType == MailboxOwnerType.User);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // get the message
        var messageModel = await _messagesContext.Client.GetMessage(
            mailboxId: messageModelFromContext.MailboxId,
            messageId: messageModelFromContext.MessageId);

        Assert.Multiple(() =>
        {
            Assert.That(messageModel.MailboxId, Is.EqualTo(messageModelFromContext.MailboxId));
            Assert.That(messageModel.MessageId, Is.EqualTo(messageModelFromContext.MessageId));
            Assert.That(messageModel.Contents, Is.EqualTo(contents));
        });
    }

    [Then(@"Messages in UserMailbox for (.*) are valid")]
    public async void UserMessagesAreValid(
        string userName,
        Table table)
    {
        var expectedTable = table.Rows.Select(row => row["Contents"]).ToArray();

        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == userModel.Id && mm.OwnerType == MailboxOwnerType.User);

        // get the messages
        var messageModels = await _messagesContext.Client.GetMessages(
            mailboxId: mailboxModel.Id);

        var actualTable = messageModels.Select(mm => mm.Contents).ToArray();

        Assert.That(actualTable, Is.EqualTo(expectedTable));
    }

    [Then(@"Message with contents '(.*)' in UserMailbox for (.*) is not found")]
    public void UserMessageIsNotFound(
        string contents,
        string userName)
    {
        // get the user
        var userModel = _usersContext.Single(um => um.UserName == userName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == userModel.Id && mm.OwnerType == MailboxOwnerType.User);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // get the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.GetMessage(
            mailboxId: messageModelFromContext.MailboxId,
            messageId: messageModelFromContext.MessageId));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Given(@"Message with contents '(.*)' in GroupMailbox for (.*) exists")]
    [When(@"Message with contents '(.*)' in GroupMailbox for (.*) is created")]
    public async void GroupMessageCreated(
        string contents,
        string groupName)
    {
        if (_messagesContext.Exists(mm => mm.Contents == contents))
        {
            return;
        }

        // get the group
        var groupModel = _groupsContext.Single(um => um.GroupName == groupName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == groupModel.Id && mm.OwnerType == MailboxOwnerType.Group);

        // create the request
        var request = new CreateMessageRequest()
        {
            MailboxId = mailboxModel.Id,
            Contents = contents
        };

        // create the message
        var messageModel = await _messagesContext.Client.CreateMessage(
            mailboxId: mailboxModel.Id,
            request: request);

        _messagesContext.Add(messageModel);
    }

    [Then(@"Messages in GroupMailbox for (.*) are valid")]
    public async void GroupMessagesAreValid(
        string groupName,
        Table table)
    {
        var expectedTable = table.Rows.Select(row => row["Contents"]).ToArray();

        // get the group
        var groupModel = _groupsContext.Single(um => um.GroupName == groupName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == groupModel.Id && mm.OwnerType == MailboxOwnerType.Group);

        // get the messages
        var messageModels = await _messagesContext.Client.GetMessages(
            mailboxId: mailboxModel.Id);

        var actualTable = messageModels.Select(mm => mm.Contents).ToArray();

        Assert.That(actualTable, Is.EqualTo(expectedTable));
    }

    [When(@"Message with contents '(.*)' in GroupMailbox for (.*) is updated to contents '(.*)'")]
    public async void GroupMessageUpdated(
        string contents,
        string groupName,
        string updatedContents)
    {
        // get the group
        var groupModel = _groupsContext.Single(um => um.GroupName == groupName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == groupModel.Id && mm.OwnerType == MailboxOwnerType.Group);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // create the request
        var request = new UpdateMessageRequest()
        {
            MailboxId = mailboxModel.Id,
            MessageId = messageModelFromContext.MessageId,
            Contents = updatedContents
        };

        // update the message
        var messageModel = await _messagesContext.Client.UpdateMessage(
            mailboxId: messageModelFromContext.MailboxId,
            messageId: messageModelFromContext.MessageId,
            request: request);

        _messagesContext.Add(messageModel);
    }

    [When(@"Message with contents '(.*)' in GroupMailbox for (.*) is deleted")]
    public async void GroupMessageDeleted(
        string contents,
        string groupName)
    {
        // get the group
        var groupModel = _groupsContext.Single(um => um.GroupName == groupName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == groupModel.Id && mm.OwnerType == MailboxOwnerType.Group);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // delete the message
        var messageModel = await _messagesContext.Client.DeleteMessage(
            mailboxId: messageModelFromContext.MailboxId,
            messageId: messageModelFromContext.MessageId);
    }

    [Then(@"Message with contents '(.*)' in GroupMailbox for (.*) is valid")]
    public async void GroupMessageIsValid(
        string contents,
        string groupName)
    {
        // get the group
        var groupModel = _groupsContext.Single(um => um.GroupName == groupName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == groupModel.Id && mm.OwnerType == MailboxOwnerType.Group);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // get the message
        var messageModel = await _messagesContext.Client.GetMessage(
            mailboxId: messageModelFromContext.MailboxId,
            messageId: messageModelFromContext.MessageId);

        Assert.Multiple(() =>
        {
            Assert.That(messageModel.MailboxId, Is.EqualTo(messageModelFromContext.MailboxId));
            Assert.That(messageModel.MessageId, Is.EqualTo(messageModelFromContext.MessageId));
            Assert.That(messageModel.Contents, Is.EqualTo(contents));
        });
    }

    [Then(@"Message with contents '(.*)' in GroupMailbox for (.*) is not found")]
    public void GroupMessageIsNotFound(
        string contents,
        string groupName)
    {
        // get the group
        var groupModel = _groupsContext.Single(um => um.GroupName == groupName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Single(mm => mm.OwnerId == groupModel.Id && mm.OwnerType == MailboxOwnerType.Group);

        // get the message
        var messageModelFromContext = _messagesContext.Single(mm => mm.MailboxId == mailboxModel.Id && mm.Contents == contents);

        // get the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.GetMessage(
                mailboxId: messageModelFromContext.MailboxId,
                messageId: messageModelFromContext.MessageId));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Then(@"CreateMessage with bad mailboxId throws BadRequest")]
    public void CreateMessageBadMailboxId()
    {
        var mailboxId1 = Guid.NewGuid();
        var mailboxId2 = Guid.NewGuid();

        // create the request
        var request = new CreateMessageRequest()
        {
            MailboxId = mailboxId2,
            Contents = string.Empty
        };

        // create the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.CreateMessage(
                mailboxId: mailboxId1,
                request: request));

        Assert.Multiple(() =>
        {
            Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(ex.Message, Is.EqualTo("Mismatch on 'mailboxId' between path and body."));
        });
    }

    [Then(@"UpdateMessage with bad mailboxId throws BadRequest")]
    public void UpdateMessageBadMailboxId()
    {
        var mailboxId1 = Guid.NewGuid();
        var mailboxId2 = Guid.NewGuid();

        var messageId = Guid.NewGuid();

        // create the request
        var request = new UpdateMessageRequest()
        {
            MailboxId = mailboxId2,
            MessageId = messageId,
            Contents = string.Empty
        };

        // update the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.UpdateMessage(
                mailboxId: mailboxId1,
                messageId: messageId,
                request: request));

        Assert.Multiple(() =>
        {
            Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(ex.Message, Is.EqualTo("Mismatch on 'mailboxId' between path and body."));
        });
    }

    [Then(@"UpdateMessage with bad messageId throws BadRequest")]
    public void UpdateMessageBadMessageId()
    {
        var mailboxId = Guid.NewGuid();

        var messageId1 = Guid.NewGuid();
        var messageId2 = Guid.NewGuid();

        // create the request
        var request = new UpdateMessageRequest()
        {
            MailboxId = mailboxId,
            MessageId = messageId2,
            Contents = string.Empty
        };

        // update the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.UpdateMessage(
                mailboxId: mailboxId,
                messageId: messageId1,
                request: request));

        Assert.Multiple(() =>
        {
            Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(ex.Message, Is.EqualTo("Mismatch on 'messageId' between path and body."));
        });
    }

    [Then(@"UpdateMessage not found")]
    public void UpdateMessageNotFound()
    {
        var mailboxId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        // create the request
        var request = new UpdateMessageRequest()
        {
            MailboxId = mailboxId,
            MessageId = messageId,
            Contents = string.Empty
        };

        // update the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.UpdateMessage(
                mailboxId: mailboxId,
                messageId: messageId,
                request: request));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Then(@"DeleteMessage not found")]
    public void DeleteMessageNotFound()
    {
        var mailboxId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        // delete the message
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _messagesContext.Client.DeleteMessage(
                mailboxId: mailboxId,
                messageId: messageId));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
