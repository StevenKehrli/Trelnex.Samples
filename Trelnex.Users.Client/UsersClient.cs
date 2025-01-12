using Trelnex.Core.Client;

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
/// <param name="httpClientFactory">The specified <see cref="IHttpClientFactory"/> to create and configure an <see cref="HttpClient"/> instance.</param>
/// <param name="getAuthorizationHeader">The specified function to get the authorization header.</param>
/// <param name="baseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
public class UsersClient(
    IHttpClientFactory httpClientFactory,
    Func<string> getAuthorizationHeader,
    Uri baseUri)
    : BaseClient(httpClientFactory), IUsersClient
{
    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    public static string Name => "Users";

    /// <summary>
    /// Creates the specified user.
    /// </summary>
    /// <param name="request">The <see cref="CreateUserRequest"/>.</param>
    /// <returns>The new <see cref="UserModel"/>.</returns>
    public async Task<UserModel> CreateUser(
        CreateUserRequest request)
    {
        return await Post<CreateUserRequest, UserModel>(
            uri: baseUri.AppendPath($"/users"),
            content: request,
            addHeaders: headers => headers.AddAuthorizationHeader(getAuthorizationHeader));
    }

    /// <summary>
    /// Gets the specified user, if it exists.
    /// </summary>
    /// <param name="userId">The specified user id.</param>
    /// <returns>The <see cref="UserModel"/>, if it exists.</returns>
    public async Task<UserModel> GetUser(
        Guid userId)
    {
        return await Get<UserModel>(
            uri: baseUri.AppendPath($"/users/{userId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(getAuthorizationHeader));
    }

    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    protected override string GetName() => Name;
}
