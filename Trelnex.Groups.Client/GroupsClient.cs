using Trelnex.Core.Client;
using Trelnex.Core.Identity;

namespace Trelnex.Groups.Client;

/// <summary>
/// Defines the contract for a Groups client.
/// </summary>
public interface IGroupsClient
{
    /// <summary>
    /// Creates the specified group.
    /// </summary>
    /// <param name="request">The <see cref="CreateGroupRequest"/>.</param>
    /// <returns>The new <see cref="GroupModel"/>.</returns>
    Task<GroupModel> CreateGroup(
        CreateGroupRequest request);

    /// <summary>
    /// Gets the specified group, if it exists.
    /// </summary>
    /// <param name="groupId">The specified group id.</param>
    /// <returns>The <see cref="GroupModel"/>, if it exists.</returns>
    Task<GroupModel> GetGroup(
        Guid groupId);
}

/// <summary>
/// Initializes a new instance of the <see cref="GroupsClient"/>.
/// </summary>
/// <param name="httpClient">The specified <see cref="HttpClient"/> instance.</param>
/// <param name="tokenProvider">The specified <see cref="IAccessTokenProvider"/> to get the access token.</param>
internal class GroupsClient(
    HttpClient httpClient,
    IAccessTokenProvider<GroupsClient> tokenProvider)
    : BaseClient(httpClient), IGroupsClient
{
    /// <summary>
    /// Creates the specified group.
    /// </summary>
    /// <param name="request">The <see cref="CreateGroupRequest"/>.</param>
    /// <returns>The new <see cref="GroupModel"/>.</returns>
    public async Task<GroupModel> CreateGroup(
        CreateGroupRequest request)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Post<CreateGroupRequest, GroupModel>(
            uri: BaseAddress.AppendPath($"/groups"),
            content: request,
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Gets the specified group, if it exists.
    /// </summary>
    /// <param name="groupId">The specified group id.</param>
    /// <returns>The <see cref="GroupModel"/>, if it exists.</returns>
    public async Task<GroupModel> GetGroup(
        Guid groupId)
    {
        var authorizationHeader = tokenProvider.GetAccessToken().GetAuthorizationHeader();

        return await Get<GroupModel>(
            uri: BaseAddress.AppendPath($"/groups/{groupId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }
}
