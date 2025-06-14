using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Data;
using Trelnex.Core.Disposables;
using Trelnex.Messages.Api.Items;
using Trelnex.Messages.Client;
using Trelnex.Users.Client;

namespace Trelnex.Messages.Api.Endpoints;

internal static class GetMessagesEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/users/{userId:guid}/messages",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesReadPolicy>()
            .Produces<MessageModel[]>()
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithName("GetMessages")
            .WithDescription("Gets the messages for the specified user")
            .WithTags("Messages")
            .ValidateUser(efiContext =>
            {
                // get our request parameters
                var usersClient = efiContext.Arguments.OfType<IUsersClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (usersClient, requestParameters.UserId);
            });
    }

    public static async Task<MessageModel[]> HandleRequest(
        [FromServices] IUsersClient usersClient,
        [FromServices] IDataProvider<IMessageItem> messageProvider,
        [AsParameters] RequestParameters parameters)
    {
        // create a query for all messages for the user
        var messageQueryCommand = messageProvider.Query()
            .Where(i => i.PartitionKey == parameters.UserId.ToString())
            .OrderBy(i => i.CreatedDateTimeOffset);

        using var messageQueryResults = await messageQueryCommand
            .ToDisposableEnumerableAsync();

        // return the message models
        return messageQueryResults.Select(mrr =>
            {
                return mrr.Item.ConvertToModel();
            })
            .ToArray();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "userId")]
        [SwaggerParameter(Description = "The specified user id", Required = true)]
        public required Guid UserId { get; init; }
    }
}
