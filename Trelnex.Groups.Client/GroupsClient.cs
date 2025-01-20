using Microsoft.Extensions.DependencyInjection;
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
/// <param name="httpClientFactory">The specified <see cref="IHttpClientFactory"/> to create and configure an <see cref="HttpClient"/> instance.</param>
/// <param name="tokenProvider">The specified <see cref="IAccessTokenProvider"/> to get the access token.</param>
/// <param name="baseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
internal class GroupsClient(
    IHttpClientFactory httpClientFactory,
    [FromKeyedServices(GroupsClient.Name)] IAccessTokenProvider tokenProvider,
    [FromKeyedServices(GroupsClient.Name)] Uri baseUri)
    : BaseClient(httpClientFactory), IGroupsClient
{
    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    public const string Name = "Groups";

    /// <summary>
    /// Creates the specified group.
    /// </summary>
    /// <param name="request">The <see cref="CreateGroupRequest"/>.</param>
    /// <returns>The new <see cref="GroupModel"/>.</returns>
    public async Task<GroupModel> CreateGroup(
        CreateGroupRequest request)
    {
        var authorizationHeader = tokenProvider.GetAuthorizationHeader();

        return await Post<CreateGroupRequest, GroupModel>(
            uri: baseUri.AppendPath($"/groups"),
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
        var authorizationHeader = tokenProvider.GetAuthorizationHeader();

        return await Get<GroupModel>(
            uri: baseUri.AppendPath($"/groups/{groupId}"),
            addHeaders: headers => headers.AddAuthorizationHeader(authorizationHeader));
    }

    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    protected override string GetName() => Name;
}
