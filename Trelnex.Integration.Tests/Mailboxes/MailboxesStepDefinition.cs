using System.Net;
using Reqnroll;
using Trelnex.Core;
using Trelnex.Integration.Tests.Groups;
using Trelnex.Integration.Tests.Users;
using Trelnex.Mailboxes.Client;

namespace Trelnex.Integration.Tests.Mailboxes;

[Binding]
public class MailboxesStepDefinitions
{
    private UsersContext _usersContext = null!;
    private GroupsContext _groupsContext = null!;
    private MailboxesContext _mailboxesContext = null!;

    [BeforeScenario("mailboxes")]
    public void BeforeScenario(
        UsersContext usersContext,
        GroupsContext groupsContext,
        MailboxesContext mailboxesContext)
    {
        _usersContext = usersContext;
        _groupsContext = groupsContext;
        _mailboxesContext = mailboxesContext;
    }

    [AfterScenario("mailboxes")]
    public void AfterScenario()
    {
        _usersContext = null!;
        _groupsContext = null!;
        _mailboxesContext = null!;
    }

    [Given(@"UserMailbox for (.*) exists")]
    public void UserMailboxCreated(
        string userName)
    {
        var userModel = _usersContext.Single(gm => gm.UserName == userName);

        // create the mailbox
        var mailboxModel = _mailboxesContext.Client.GetUserMailbox(userModel.Id).Result;

        _mailboxesContext.Add(mailboxModel);
    }

    [Then(@"UserMailbox for (.*) is valid")]
    public void UserMailboxIsValid(
        string userName)
    {
        var userModel = _usersContext.Single(gm => gm.UserName == userName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Client.GetUserMailbox(userModel.Id).Result;

        Assert.Multiple(() =>
        {
            Assert.That(mailboxModel.OwnerId, Is.EqualTo(userModel.Id));
            Assert.That(mailboxModel.OwnerType, Is.EqualTo(MailboxOwnerType.User));
        });
    }

    [Given(@"GroupMailbox for (.*) exists")]
    public void GroupMailboxCreated(
        string groupName)
    {
        var groupModel = _groupsContext.Single(gm => gm.GroupName == groupName);

        // create the mailbox
        var mailboxModel = _mailboxesContext.Client.GetGroupMailbox(groupModel.Id).Result;

        _mailboxesContext.Add(mailboxModel);
    }

    [Then(@"GroupMailbox for (.*) is valid")]
    public void GroupMailboxIsValid(
        string groupName)
    {
        var groupModel = _groupsContext.Single(gm => gm.GroupName == groupName);

        // get the mailbox
        var mailboxModel = _mailboxesContext.Client.GetGroupMailbox(groupModel.Id).Result;

        Assert.Multiple(() =>
        {
            Assert.That(mailboxModel.OwnerId, Is.EqualTo(groupModel.Id));
            Assert.That(mailboxModel.OwnerType, Is.EqualTo(MailboxOwnerType.Group));
        });
    }

    [Then(@"Mailbox (.*) is not found")]
    public void MailboxIsNotFound(
        Guid mailboxId)
    {
        // create the mailbox
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _mailboxesContext.Client.GetMailbox(mailboxId));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
