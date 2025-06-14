using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Data;
using Trelnex.Messages.Api.Items;
using Trelnex.Messages.Client;
using Trelnex.Users.Client;

namespace Trelnex.Messages.Api.Endpoints;

internal static class DeleteMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapDelete(
                "/users/{userId:guid}/messages/{messageId:guid}",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesDeletePolicy>()
            .Produces<DeleteMessageResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithName("DeleteMessage")
            .WithDescription("Deletes the specified message")
            .WithTags("Messages")
            .ValidateUser(efiContext =>
            {
                // get our request parameters
                var usersClient = efiContext.Arguments.OfType<IUsersClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (usersClient, requestParameters.UserId);
            });
    }

    public static async Task<DeleteMessageResponse> HandleRequest(
        [FromServices] IUsersClient usersClient,
        [FromServices] IDataProvider<IMessageItem> messageProvider,
        [AsParameters] RequestParameters parameters)
    {
        // delete the message item
        using var messageDeleteCommand = await messageProvider.DeleteAsync(
            id: parameters.MessageId.ToString(),
            partitionKey: parameters.UserId.ToString());

        // throw if not found
        if (messageDeleteCommand is null) throw new HttpStatusCodeException(HttpStatusCode.NotFound);

        // save in data store
        using var messageDeleteResult = await messageDeleteCommand.SaveAsync(default);

        // return the delete response
        return new DeleteMessageResponse
        {
            UserId = Guid.Parse(messageDeleteResult.Item.PartitionKey),
            MessageId = Guid.Parse(messageDeleteResult.Item.Id),
            DeletedDateTimeOffset = messageDeleteResult.Item.DeletedDateTimeOffset!.Value,
        };
    }

    public class RequestParameters
    {
        [FromRoute(Name = "userId")]
        [SwaggerParameter(Description = "The specified user id", Required = true)]
        public required Guid UserId { get; init; }

        [FromRoute(Name = "messageId")]
        [SwaggerParameter(Description = "The specified message id", Required = true)]
        public required Guid MessageId { get; init; }
    }
}
