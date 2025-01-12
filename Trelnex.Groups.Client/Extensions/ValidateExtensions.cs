using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Trelnex.Core;

namespace Trelnex.Groups.Client;

/// <summary>
/// Extension method to add an <see cref="IEndpointFilter"/> to validate a group.
/// </summary>
public static class ValidateGroupExtensions
{
    /// <summary>
    /// Adds an <see cref="IEndpointFilter"/> to the <see cref="RouteHandlerBuilder"/> to validate a group.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to add the <see cref="IEndpointFilter"/> to.</param>
    /// <param name="getClientAndGroupId">The delegate to get the <see cref="IGroupsClient"/> and group id.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/>.</returns>
    public static RouteHandlerBuilder ValidateGroup(
        this RouteHandlerBuilder builder,
        Func<EndpointFilterInvocationContext, (IGroupsClient groupsClient, Guid groupId)> getClientAndGroupId)
    {
        builder.AddEndpointFilter(async (efiContext, next) =>
        {
            // get the groups client and group id
            var result = getClientAndGroupId(efiContext);

            var groupsClient = result.groupsClient;
            var groupId = result.groupId;

            // validate
            return await ValidateGroup(groupsClient, groupId)
                ? await next(efiContext)
                : throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        });

        return builder;
    }

    /// <summary>
    /// Validates the specified group.
    /// </summary>
    /// <param name="groupsClient">The <see cref="IGroupsClient"/>.</param>
    /// <param name="groupId">The specified group id.</param>
    /// <returns>true if the group exists; otherwise, false.</returns>
    private static async Task<bool> ValidateGroup(
        IGroupsClient groupsClient,
        Guid groupId)
    {
        try
        {
            await groupsClient.GetGroup(groupId);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
