using Trelnex.Core.Data;
using Trelnex.Groups.Client;
using Trelnex.Mailboxes.Api.Endpoints;
using Trelnex.Mailboxes.Api.Objects;
using Trelnex.Mailboxes.Client;
using Trelnex.Users.Client;

namespace Trelnex.Integration.Tests.Mailboxes;

internal class MailboxesClient(
    IUsersClient usersClient,
    IGroupsClient groupsClient,
    ICommandProvider<IMailbox> mailboxProvider,
    ICommandProvider<IUserMailbox> userMailboxProvider,
    ICommandProvider<IGroupMailbox> groupMailboxProvider) : IMailboxesClient
{
    public async Task<MailboxModel> GetGroupMailbox(
        Guid groupId)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new GetGroupMailboxEndpoint.RequestParameters()
        {
            GroupId = groupId
        };

        // invoke
        return await GetGroupMailboxEndpoint.HandleRequest(
            groupsClient: groupsClient,
            mailboxProvider: mailboxProvider,
            groupMailboxProvider: groupMailboxProvider,
            requestContext: requestContext,
            parameters: parameters);
    }

    public async Task<MailboxModel> GetMailbox(
        Guid mailboxId)
    {
        // format the request arguments
        var parameters = new GetMailboxEndpoint.RequestParameters()
        {
            MailboxId = mailboxId
        };

        // invoke
        return await GetMailboxEndpoint.HandleRequest(
            mailboxProvider: mailboxProvider,
            parameters: parameters);
    }

    public async Task<MailboxModel> GetUserMailbox(
        Guid userId)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new GetUserMailboxEndpoint.RequestParameters()
        {
            UserId = userId
        };

        // invoke
        return await GetUserMailboxEndpoint.HandleRequest(
            usersClient: usersClient,
            mailboxProvider: mailboxProvider,
            userMailboxProvider: userMailboxProvider,
            requestContext: requestContext,
            parameters: parameters);
    }
}
