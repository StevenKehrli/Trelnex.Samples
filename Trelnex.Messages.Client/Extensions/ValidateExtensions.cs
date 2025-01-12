using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Trelnex.Core;

namespace Trelnex.Messages.Client;

/// <summary>
/// Extension method to add an <see cref="IEndpointFilter"/> to validate a message.
/// </summary>
public static class ValidateExtensions
{
    /// <summary>
    /// Adds an <see cref="IEndpointFilter"/> to the <see cref="RouteHandlerBuilder"/> to validate a message.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to add the <see cref="IEndpointFilter"/> to.</param>
    /// <param name="getClientAndMessageId">The delegate to get the <see cref="IMessagesClient"/> and message id.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/>.</returns>
    public static RouteHandlerBuilder ValidateMessage(
        this RouteHandlerBuilder builder,
        Func<EndpointFilterInvocationContext, (IMessagesClient messagesClient, Guid mailboxId, Guid messageId)> getClientAndMessageId)
    {
        builder.AddEndpointFilter(async (efiContext, next) =>
        {
            // get the messages client and message id
            var result = getClientAndMessageId(efiContext);

            var messagesClient = result.messagesClient;
            var mailboxId = result.mailboxId;
            var messageId = result.messageId;

            // validate
            return await ValidateMessage(messagesClient, mailboxId, messageId)
                ? await next(efiContext)
                : throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        });

        return builder;
    }

    /// <summary>
    /// Validates the specified message.
    /// </summary>
    /// <param name="messagesClient">The <see cref="IMessagesClient"/>.</param>
    /// <param name="mailboxId">The specified mailbox id.</param>
    /// <param name="messageId">The specified message id.</param>
    /// <returns>true if the message exists; otherwise, false.</returns>
    private static async Task<bool> ValidateMessage(
        IMessagesClient messagesClient,
        Guid mailboxId,
        Guid messageId)
    {
        try
        {
            await messagesClient.GetMessage(mailboxId, messageId);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
