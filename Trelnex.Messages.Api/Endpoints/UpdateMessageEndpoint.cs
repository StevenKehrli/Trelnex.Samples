using System.Net;
using System.Net.Mime;
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

internal static class UpdateMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapPut(
                "/mailboxes/{mailboxId:guid}/messages/{messageId:guid}",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesUpdatePolicy>()
            .Accepts<UpdateMessageRequest>(MediaTypeNames.Application.Json)
            .Produces<MessageModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("UpdateMessage")
            .WithDescription("Updates a message")
            .WithTags("Messages")
            .ValidateMailbox(efiContext =>
            {
                // get our request parameters
                var mailboxesClient = efiContext.Arguments.OfType<IMailboxesClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (mailboxesClient, requestParameters.MailboxId);
            });
    }

    public static async Task<MessageModel> HandleRequest(
        [FromServices] IMailboxesClient mailboxesClient,
        [FromServices] ICommandProvider<IMessage> messageProvider,
        [FromServices] IRequestContext requestContext,
        [AsParameters] RequestParameters parameters)
    {
        // validate the mailbox id from the route and the mailbox id from the request
        if (parameters.MailboxId != parameters.Request.MailboxId)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Mismatch on 'mailboxId' between path and body.");
        }

        // validate the message id from the route and the message id from the request
        if (parameters.MessageId != parameters.Request.MessageId)
        {
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Mismatch on 'messageId' between path and body.");
        }

        // update the message dto
        var messageUpdateCommand = await messageProvider.UpdateAsync(
            id: parameters.MessageId.ToString(),
            partitionKey: parameters.MailboxId.ToString());

        // throw if not found
        if (messageUpdateCommand is null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        messageUpdateCommand.Item.Contents = parameters.Request.Contents;

        // save in data store
        var messageUpdateResult = await messageUpdateCommand.SaveAsync(requestContext, default);

        // return the message model
        return messageUpdateResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "mailboxId")]
        [SwaggerParameter(Description = "The specified mailbox id", Required = true)]
        public required Guid MailboxId { get; init; }

        [FromRoute(Name = "messageId")]
        [SwaggerParameter(Description = "The specified message id", Required = true)]
        public required Guid MessageId { get; init; }

        [FromBody]
        public required UpdateMessageRequest Request { get; init; }
    }
}
