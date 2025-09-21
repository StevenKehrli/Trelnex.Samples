using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core.Exceptions;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Data;
using Trelnex.Messages.Api.Items;
using Trelnex.Messages.Client;
using Trelnex.Users.Client;

namespace Trelnex.Messages.Api.Endpoints;

internal static class GetMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/users/{userId:guid}/messages/{messageId:guid}",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesReadPolicy>()
            .Produces<MessageModel>()
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithName("GetMessage")
            .WithDescription("Gets the specified message")
            .WithTags("Messages")
            .ValidateUser(efiContext =>
            {
                // get our request parameters
                var usersClient = efiContext.Arguments.OfType<IUsersClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (usersClient, requestParameters.UserId);
            });
    }

    public static async Task<MessageModel> HandleRequest(
        [FromServices] IUsersClient usersClient,
        [FromServices] IDataProvider<MessageItem> messageProvider,
        [AsParameters] RequestParameters parameters)
    {
        // get the message item from data store
        using var messageReadResult = await messageProvider.ReadAsync(
            id: parameters.MessageId.ToString(),
            partitionKey: parameters.UserId.ToString());

        // throw if not found
        if (messageReadResult is null) throw new HttpStatusCodeException(HttpStatusCode.NotFound);

        // return the message model
        return messageReadResult.Item.ConvertToModel();
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
