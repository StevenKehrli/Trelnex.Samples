using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Client;
using Trelnex.Messages.Api.Objects;
using Trelnex.Messages.Client;

namespace Trelnex.Messages.Api.Endpoints;

internal static class GetMessagesEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/mailboxes/{mailboxId:guid}/messages",
                HandleRequest)
            // .RequirePermission<MessagesPermission.MessagesReadPolicy>()
            .Produces<MessageModel[]>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("GetMessages")
            .WithDescription("Gets the messages for the specified mailbox")
            .WithTags("Messages")
            .ValidateMailbox(efiContext =>
            {
                // get our request parameters
                var mailboxesClient = efiContext.Arguments.OfType<IMailboxesClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (mailboxesClient, requestParameters.MailboxId);
            });
    }

    public static async Task<MessageModel[]> HandleRequest(
        [FromServices] IMailboxesClient mailboxesClient,
        [FromServices] ICommandProvider<IMessage> messageProvider,
        [AsParameters] RequestParameters parameters)
    {
        // create a query for all messages in the mailbox
        var messageQueryCommand = messageProvider.Query()
            .Where(i => i.PartitionKey == parameters.MailboxId.ToString())
            .OrderBy(i => i.CreatedDate);

        var messageQueryResults = await messageQueryCommand.ToAsyncEnumerable().ToArrayAsync();

        // return the message models
        return messageQueryResults.Select(mrr =>
            mrr.Item.ConvertToModel())
            .ToArray();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "mailboxId")]
        [SwaggerParameter(Description = "The specified mailbox id", Required = true)]
        public required Guid MailboxId { get; init; }
    }
}
