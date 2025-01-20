using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Users.Api.Objects;
using Trelnex.Users.Client;

namespace Trelnex.Users.Api.Endpoints;

internal static class CreateUserEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapPost(
                "/users",
                HandleRequest)
            .RequirePermission<UsersPermission.UsersCreatePolicy>()
            .Accepts<CreateUserRequest>(MediaTypeNames.Application.Json)
            .Produces<UserModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("CreateUser")
            .WithDescription("Creates a new user")
            .WithTags("Users");
    }

    public static async Task<UserModel> HandleRequest(
        [FromServices] ICommandProvider<IUser> userProvider,
        [FromServices] IRequestContext requestContext,
        [AsParameters] RequestParameters parameters)
    {
        // create a new user id
        var id = Guid.NewGuid().ToString();
        var partitionKey = id;

        // create the user dto
        var userCreateCommand = userProvider.Create(
            id: id,
            partitionKey: partitionKey);

        userCreateCommand.Item.UserName = parameters.Request.UserName;

        // save in data store
        var userCreateResult = await userCreateCommand.SaveAsync(requestContext, default);

        // return the user model
        return userCreateResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromBody]
        public required CreateUserRequest Request { get; init; }
    }
}
