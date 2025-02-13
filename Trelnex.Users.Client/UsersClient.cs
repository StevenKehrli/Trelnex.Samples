using Microsoft.Extensions.DependencyInjection;
using Trelnex.Core.Client;
using Trelnex.Core.Identity;

namespace Trelnex.Users.Client;

/// <summary>
/// Defines the contract for a Users client.
/// </summary>
public interface IUsersClient
{
    /// <summary>
    /// Creates the specified user.
    /// </summary>
    /// <param name="request">The <see cref="CreateUserRequest"/>.</param>
    /// <returns>The new <see cref="UserModel"/>.</returns>
    Task<UserModel> CreateUser(
        CreateUserRequest request);

    /// <summary>
    /// Gets the specified user, if it exists.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The <see cref="UserModel"/>, if it exists.</returns>
    Task<UserModel> GetUser(
        Guid userId);
}

/// <summary>
/// Initializes a new instance of the <see cref="UsersClient"/>.
/// </summary>
/// <param name="httpClient">The specified <see cref="HttpClient"/> instance.</param>
/// <param name="tokenProvider">The specified <see cref="IAccessTokenProvider"/> to get the access token.</param>
internal class UsersClient(
    HttpClient httpClient,
    IAccessTokenProvider<UsersClient> tokenProvider)
    : BaseClient(httpClient), IUsersClient
{
    /// <summary>
    /// Creates the specified user.
    /// </summary>
    /// <param name="request">The <see cref="CreateUserRequest"/>.</param>
    /// <returns>The new <see cref="UserModel"/>.</returns>
    public async Task<UserModel> CreateUser(
        CreateUserRequest request)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Post<CreateUserRequest, UserModel>(
            uri: BaseAddress.AppendPath($"/users"),
            content: request,
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Gets the specified user, if it exists.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The <see cref="UserModel"/>, if it exists.</returns>
    public async Task<UserModel> GetUser(
        Guid userId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Get<UserModel>(
            uri: BaseAddress.AppendPath($"/users/{userId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }
}
