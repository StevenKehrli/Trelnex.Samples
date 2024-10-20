using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Api.Objects;
using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Endpoints;

internal static class GetMailboxEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/mailboxes/{mailboxId:guid}",
                HandleRequest)
            .RequirePermission<MailboxesPermission.MailboxesReadPolicy>()
            .Produces(StatusCodes.Status200OK)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("GetMailbox")
            .WithDescription("Gets the mailbox with the specified id.")
            .WithTags("Mailboxes");
    }

    public static async Task<MailboxModel> HandleRequest(
        [FromServices] ICommandProvider<IMailbox> mailboxProvider,
        [AsParameters] RequestParameters parameters)
    {
        // get the mailbox dto from data store
        var mailboxReadResult = await mailboxProvider.ReadAsync(
            id: parameters.MailboxId.ToString(),
            partitionKey: parameters.MailboxId.ToString());

        // throw if not found
        if (mailboxReadResult is null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        // return mailbox model
        return mailboxReadResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "mailboxId")]
        [SwaggerParameter(Description = "The specified mailbox id.", Required = true)]
        public required Guid MailboxId { get; init; }
    }
}
