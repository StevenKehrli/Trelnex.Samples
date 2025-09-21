using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Trelnex.Core.Exceptions;

namespace Trelnex.Users.Client;

/// <summary>
/// Extension method to add an <see cref="IEndpointFilter"/> to validate a user.
/// </summary>
public static class ValidateExtensions
{
    /// <summary>
    /// Adds an <see cref="IEndpointFilter"/> to the <see cref="RouteHandlerBuilder"/> to validate a user.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to add the <see cref="IEndpointFilter"/> to.</param>
    /// <param name="getClientAndUserId">The delegate to get the <see cref="IUsersClient"/> and user id.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/>.</returns>
    public static RouteHandlerBuilder ValidateUser(
        this RouteHandlerBuilder builder,
        Func<EndpointFilterInvocationContext, (IUsersClient usersClient, Guid userId)> getClientAndUserId)
    {
        builder.AddEndpointFilter(async (efiContext, next) =>
        {
            // get the users client and user id
            var result = getClientAndUserId(efiContext);

            var usersClient = result.usersClient;
            var userId = result.userId;

            // validate
            return await ValidateUser(usersClient, userId)
                ? await next(efiContext)
                : throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        });

        return builder;
    }

    /// <summary>
    /// Validates the specified user.
    /// </summary>
    /// <param name="usersClient">The <see cref="IUsersClient"/>.</param>
    /// <param name="userId">The specified user id.</param>
    /// <returns>true if the user exists; otherwise, false.</returns>
    private static async Task<bool> ValidateUser(
        IUsersClient usersClient,
        Guid userId)
    {
        try
        {
            await usersClient.GetUser(userId);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
