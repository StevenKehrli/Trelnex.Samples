using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Trelnex.Core;

namespace Trelnex.Mailboxes.Client;

/// <summary>
/// Extension method to add an <see cref="IEndpointFilter"/> to validate a mailbox.
/// </summary>
public static class ValidateExtensions
{
    /// <summary>
    /// Adds an <see cref="IEndpointFilter"/> to the <see cref="RouteHandlerBuilder"/> to validate a mailbox.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to add the <see cref="IEndpointFilter"/> to.</param>
    /// <param name="getClientAndMailboxId">The delegate to get the <see cref="IMailboxesClient"/> and mailbox id.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/>.</returns>
    public static RouteHandlerBuilder ValidateMailbox(
        this RouteHandlerBuilder builder,
        Func<EndpointFilterInvocationContext, (IMailboxesClient, Guid)> getClientAndMailboxId)
    {
        builder.AddEndpointFilter(async (efiContext, next) =>
        {
            // get the mailboxes client and mailbox id
            var result = getClientAndMailboxId(efiContext);

            var mailboxesClient = result.Item1;
            var mailboxId = result.Item2;

            // validate
            return await ValidateMailbox(mailboxesClient, mailboxId)
                ? await next(efiContext)
                : throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        });

        return builder;
    }

    /// <summary>
    /// Validates the specified mailbox.
    /// </summary>
    /// <param name="mailboxesClient">The <see cref="IMailboxesClient"/>.</param>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <returns>true if the mailbox exists; otherwise, false.</returns>
    private static async Task<bool> ValidateMailbox(
        IMailboxesClient mailboxesClient,
        Guid mailboxId)
    {
        try
        {
            await mailboxesClient.GetMailbox(mailboxId);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
