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

internal static class GetMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/mailboxes/{mailboxId:guid}/messages/{messageId:guid}",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesReadPolicy>()
            .Produces<MessageModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("GetMessage")
            .WithDescription("Gets the specified message")
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
        [AsParameters] RequestParameters parameters)
    {
        // get the message dto from data store
        var messageReadResult = await messageProvider.ReadAsync(
            id: parameters.MessageId.ToString(),
            partitionKey: parameters.MailboxId.ToString());

        // throw if not found
        if (messageReadResult is null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        // return the message model
        return messageReadResult.Item.ConvertToModel();
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
