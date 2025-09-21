using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core.Exceptions;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Data;
using Trelnex.Messages.Api.Items;
using Trelnex.Messages.Client;
using Trelnex.Users.Client;

namespace Trelnex.Messages.Api.Endpoints;

internal static class UpdateMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapPut(
                "/users/{userId:guid}/messages/{messageId:guid}",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesUpdatePolicy>()
            .Accepts<UpdateMessageRequest>(MediaTypeNames.Application.Json)
            .Produces<MessageModel>()
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithName("UpdateMessage")
            .WithDescription("Updates a message")
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
        // update the message item
        using var messageUpdateCommand = await messageProvider.UpdateAsync(
            id: parameters.MessageId.ToString(),
            partitionKey: parameters.UserId.ToString());

        if (messageUpdateCommand is null) throw new HttpStatusCodeException(HttpStatusCode.NotFound);

        messageUpdateCommand.Item.Contents = parameters.Request.Contents;

        // save in data store
        var messageUpdateResult = await messageUpdateCommand.SaveAsync(default);

        // return the message model
        return messageUpdateResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "userId")]
        [SwaggerParameter(Description = "The specified user id", Required = true)]
        public required Guid UserId { get; init; }

        [FromRoute(Name = "messageId")]
        [SwaggerParameter(Description = "The specified message id", Required = true)]
        public required Guid MessageId { get; init; }

        [FromBody]
        public required UpdateMessageRequest Request { get; init; }
    }
}
