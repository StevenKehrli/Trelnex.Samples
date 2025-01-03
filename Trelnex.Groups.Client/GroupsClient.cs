using Azure.Core;
using Trelnex.Core.Client;

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
/// <param name="tokenCredential">The specified <see cref="TokenCredential"/> to get the <see cref="AccessToken"/> for the specified set of scopes.</param>
/// <param name="tokenRequestContext">The <see cref="TokenRequestContext"/> with authentication information.</param>
/// <param name="baseUri">The base <see cref="Uri"/> to build the request <see cref="Uri"/>.</param>
public class GroupsClient(
    IHttpClientFactory httpClientFactory,
    TokenCredential tokenCredential,
    TokenRequestContext tokenRequestContext,
    Uri baseUri)
    : BaseClient(httpClientFactory), IGroupsClient
{
    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    public static string Name => "Groups";

    /// <summary>
    /// Creates the specified group.
    /// </summary>
    /// <param name="request">The <see cref="CreateGroupRequest"/>.</param>
    /// <returns>The new <see cref="GroupModel"/>.</returns>
    public async Task<GroupModel> CreateGroup(
        CreateGroupRequest request)
    {
        return await Post<CreateGroupRequest, GroupModel>(
            uri: baseUri.AppendPath($"/groups"),
            content: request,
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
    }

    /// <summary>
    /// Gets the specified group, if it exists.
    /// </summary>
    /// <param name="groupId">The specified group id.</param>
    /// <returns>The <see cref="GroupModel"/>, if it exists.</returns>
    public async Task<GroupModel> GetGroup(
        Guid groupId)
    {
        return await Get<GroupModel>(
            uri: baseUri.AppendPath($"/groups/{groupId}"),
            addHeaders: headers => headers.AddBearerToken(tokenCredential, tokenRequestContext));
    }

    /// <summary>
    /// Gets the name of this client.
    /// </summary>
    /// <returns>The name of this client.</returns>
    protected override string GetName() => Name;
}
