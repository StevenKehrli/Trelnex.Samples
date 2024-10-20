using System.Net;
using TechTalk.SpecFlow;
using Trelnex.Core;
using Trelnex.Users.Client;

namespace Trelnex.Integration.Tests.Users;

[Binding]
public class UsersStepDefinitions
{
    private UsersContext _usersContext = null!;

    [BeforeScenario("users")]
    public void BeforeScenario(
        UsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    [AfterScenario("users")]
    public void AfterScenario()
    {
        _usersContext = null!;
    }

    [Given(@"User (.*) exists")]
    [When(@"User (.*) is created")]
    public async void UserCreated(
        string userName)
    {
        if (_usersContext.Exists(um => um.UserName == userName))
        {
            return;
        }

        // create the request
        var request = new CreateUserRequest()
        {
            UserName = userName
        };

        // create the user
        var userModel = await _usersContext.Client.CreateUser(request);

        _usersContext.Add(userModel);
    }

    [Then(@"User (.*) is valid")]
    public async void UserIsValid(
        string userName)
    {
        var userModelFromContext = _usersContext.Single(um => um.UserName == userName);

        // get the user
        var userModel = await _usersContext.Client.GetUser(userModelFromContext.Id);

        Assert.Multiple(() =>
        {
            Assert.That(userModel.Id, Is.Not.Default);
            Assert.That(userModel.UserName, Is.EqualTo(userName));
        });
    }

    [Then(@"User (.*) is not found")]
    public void UserIsNotFound(
        Guid userId)
    {
        // create the mailbox
        var ex = Assert.ThrowsAsync<HttpStatusCodeException>(
            async () => await _usersContext.Client.GetUser(userId));

        Assert.That(ex.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
