using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Data;
using Trelnex.Messages.Api.Items;
using Trelnex.Messages.Client;
using Trelnex.Users.Client;

namespace Trelnex.Messages.Api.Endpoints;

internal static class CreateMessageEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapPost(
                "/users/{userId:guid}/messages",
                HandleRequest)
            .RequirePermission<MessagesPermission.MessagesCreatePolicy>()
            .Accepts<CreateMessageRequest>(MediaTypeNames.Application.Json)
            .Produces<MessageModel>()
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithName("CreateMessage")
            .WithDescription("Creates a new message")
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
        // create a new message id
        var id = Guid.NewGuid().ToString();

        // create the message item
        using var messageCreateCommand = messageProvider.Create(
            id: id,
            partitionKey: parameters.UserId.ToString());

        messageCreateCommand.Item.Contents = parameters.Request.Contents;

        // save in data store
        using var messageCreateResult = await messageCreateCommand.SaveAsync(default);

        // return the message model
        return messageCreateResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "userId")]
        [SwaggerParameter(Description = "The specified user id", Required = true)]
        public required Guid UserId { get; init; }

        [FromBody]
        public required CreateMessageRequest Request { get; init; }
    }
}
