using System.Net;
using Reqnroll;
using Trelnex.Core;
using Trelnex.Groups.Client;

namespace Trelnex.Integration.Tests.Groups;

[Binding]
public class GroupsStepDefinitions
{
    private GroupsContext _groupsContext = null!;

    [BeforeScenario("groups")]
    public void BeforeScenario(
        GroupsContext groupsContext)
    {
        _groupsContext = groupsContext;
    }

    [AfterScenario("groups")]
    public void AfterScenario()
    {
        _groupsContext = null!;
    }

    [Given(@"Group (.*) exists")]
    [When(@"Group (.*) is created")]
    public void GroupCreated(
        string groupName)
    {
        if (_groupsContext.Exists(um => um.GroupName == groupName))
        {
            return;
        }

        // create the request
        var request = new CreateGroupRequest()
        {
            GroupName = groupName,
        };

        // create the group
        var groupModel = _groupsContext.Client.CreateGroup(request).Result;

        _groupsContext.Add(groupModel);
    }

    [Then(@"Group (.*) is valid")]
    public void GroupIsValid(
        string groupName)
    {
        var groupModelFromContext = _groupsContext.Single(um => um.GroupName == groupName);

        // get the group
        var groupModel = _groupsContext.Client.GetGroup(groupModelFromContext.Id).Result;

        Assert.Multiple(() =>
        {
            Assert.That(groupModel.Id, Is.Not.Default);
            Assert.That(groupModel.GroupName, Is.EqualTo(groupName));
        });
    }

    [Then(@"Group (.*) is not found")]
    public void GroupIsNotFound(
        Guid groupId)
    {
        // create the mailbox
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _groupsContext.Client.GetGroup(groupId));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
