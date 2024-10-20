using Trelnex.Core.Data;
using Trelnex.Users.Api.Endpoints;
using Trelnex.Users.Api.Objects;
using Trelnex.Users.Client;

namespace Trelnex.Integration.Tests.Users;

internal class UsersClient(
    ICommandProvider<IUser> userProvider) : IUsersClient
{
    public async Task<UserModel> CreateUser(
        CreateUserRequest request)
    {
        // create a mock request context
        var requestContext = TestRequestContext.Create();

        // format the request arguments
        var parameters = new CreateUserEndpoint.RequestParameters()
        {
            Request = request
        };

        // invoke
        return await CreateUserEndpoint.HandleRequest(
            userProvider: userProvider,
            requestContext: requestContext,
            parameters: parameters);
    }

    public async Task<UserModel> GetUser(
        Guid userId)
    {
        // format the request arguments
        var parameters = new GetUserEndpoint.RequestParameters()
        {
            UserId = userId
        };

        // invoke
        return await GetUserEndpoint.HandleRequest(
            userProvider: userProvider,
            parameters: parameters);
    }
}
