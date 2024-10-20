using Trelnex.Core.Data;
using Trelnex.Groups.Api.Endpoints;
using Trelnex.Groups.Api.Objects;
using Trelnex.Groups.Client;

namespace Trelnex.Integration.Tests.Groups;

internal class GroupsClient(
    ICommandProvider<IGroup> groupProvider) : IGroupsClient
{
    public async Task<GroupModel> CreateGroup(
        CreateGroupRequest request)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new CreateGroupEndpoint.RequestParameters()
        {
            Request = request
        };

        // invoke
        return await CreateGroupEndpoint.HandleRequest(
            groupProvider: groupProvider,
            requestContext: requestContext,
            parameters: parameters);
    }

    public async Task<GroupModel> GetGroup(
        Guid groupId)
    {
        // format the request arguments
        var parameters = new GetGroupEndpoint.RequestParameters()
        {
            GroupId = groupId
        };

        // invoke
        return await GetGroupEndpoint.HandleRequest(
            groupProvider: groupProvider,
            parameters: parameters);
    }
}
