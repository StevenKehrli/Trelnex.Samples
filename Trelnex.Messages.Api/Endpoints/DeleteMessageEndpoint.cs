using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Client;
using Trelnex.Messages.Api.Objects;
using Trelnex.Messages.Client;

namespace Trelnex.Messages.Api.Endpoints;

internal static class DeleteMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapDelete(
                "/mailboxes/{mailboxId:guid}/messages/{messageId:guid}",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesDeletePolicy>()
            .Produces<DeleteMessageResponse>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("DeleteMessage")
            .WithDescription("Deletes the specified message")
            .WithTags("Messages")
            .ValidateMailbox(efiContext =>
            {
                // get our request parameters
                var mailboxesClient = efiContext.Arguments.OfType<IMailboxesClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (mailboxesClient, requestParameters.MailboxId);
            });
    }

    public static async Task<DeleteMessageResponse> HandleRequest(
        [FromServices] IMailboxesClient mailboxesClient,
        [FromServices] ICommandProvider<IMessage> messageProvider,
        [FromServices] IRequestContext requestContext,
        [AsParameters] RequestParameters parameters)
    {
        // delete the message dto
        var messageDeleteCommand = await messageProvider.DeleteAsync(
            id: parameters.MessageId.ToString(),
            partitionKey: parameters.MailboxId.ToString());

        // throw if not found
        if (messageDeleteCommand is null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        // save in data store
        var messageDeleteResult = await messageDeleteCommand.SaveAsync(requestContext, default);

        // return the delete response
        return new DeleteMessageResponse
        {
            MailboxId = Guid.Parse(messageDeleteResult.Item.PartitionKey),
            MessageId = Guid.Parse(messageDeleteResult.Item.Id),
            DeletedDate = messageDeleteResult.Item.DeletedDate!.Value,
        };
    }

    public class RequestParameters
    {
        [FromRoute(Name = "mailboxId")]
        [SwaggerParameter(Description = "The specified mailbox id", Required = true)]
        public required Guid MailboxId { get; init; }

        [FromRoute(Name = "messageId")]
        [SwaggerParameter(Description = "The specified message id", Required = true)]
        public required Guid MessageId { get; init; }
    }
}
