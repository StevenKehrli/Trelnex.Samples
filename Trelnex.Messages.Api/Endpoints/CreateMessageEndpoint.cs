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

internal static class CreateMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapPost(
                "/mailboxes/{mailboxId:guid}/messages",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesCreatePolicy>()
            .Accepts<CreateMessageRequest>(MediaTypeNames.Application.Json)
            .Produces<MessageModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("CreateMessage")
            .WithDescription("Creates a new message")
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

        // create a new message id
        var id = Guid.NewGuid().ToString();

        // create the message dto
        var messageCreateCommand = messageProvider.Create(
            id: id,
            partitionKey: parameters.MailboxId.ToString());

        messageCreateCommand.Item.Contents = parameters.Request.Contents;

        // save in data store
        var messageCreateResult = await messageCreateCommand.SaveAsync(requestContext, default);

        // return the message model
        return messageCreateResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "mailboxId")]
        [SwaggerParameter(Description = "The specified mailbox id", Required = true)]
        public required Guid MailboxId { get; init; }

        [FromBody]
        public required CreateMessageRequest Request { get; init; }
    }
}
