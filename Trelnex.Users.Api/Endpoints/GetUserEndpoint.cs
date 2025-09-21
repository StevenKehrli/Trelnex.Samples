using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core.Exceptions;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Data;
using Trelnex.Users.Api.Items;
using Trelnex.Users.Client;

namespace Trelnex.Users.Api.Endpoints;

internal static class GetUserEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/users/{userId:guid}",
                HandleRequest)
            .RequirePermission<UsersPermission.UsersReadPolicy>()
            .Produces<UserModel>()
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithName("GetUser")
            .WithDescription("Gets the specified user")
            .WithTags("Users");
    }

    public static async Task<UserModel> HandleRequest(
        [FromServices] IDataProvider<UserItem> userProvider,
        [AsParameters] RequestParameters parameters)
    {
        // get the user item from data store
        using var userReadResult = await userProvider.ReadAsync(
            id: parameters.UserId.ToString(),
            partitionKey: parameters.UserId.ToString());

        // throw if not found
        if (userReadResult is null) throw new HttpStatusCodeException(HttpStatusCode.NotFound);

        // return the user model
        return userReadResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "userId")]
        [SwaggerParameter(Description = "The specified user id", Required = true)]
        public required Guid UserId { get; init; }
    }
}
