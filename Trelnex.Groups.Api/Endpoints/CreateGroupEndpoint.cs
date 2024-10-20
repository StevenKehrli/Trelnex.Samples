using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Trelnex.Core;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Groups.Api.Objects;
using Trelnex.Groups.Client;

namespace Trelnex.Groups.Api.Endpoints;

internal static class CreateGroupEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapPost(
                "/groups",
                HandleRequest)
            .RequirePermission<GroupsPermission.GroupsCreatePolicy>()
            .Accepts<CreateGroupRequest>(MediaTypeNames.Application.Json)
            .Produces<GroupModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("CreateGroup")
            .WithDescription("Creates a new group")
            .WithTags("Groups");
    }

    public static async Task<GroupModel> HandleRequest(
        [FromServices] ICommandProvider<IGroup> groupProvider,
        [FromServices] IRequestContext requestContext,
        [AsParameters] RequestParameters parameters)
    {
        // create a new group id
        var id = Guid.NewGuid().ToString();
        var partitionKey = id;

        // create the group dto
        var groupCreateCommand = groupProvider.Create(
            id: id,
            partitionKey: partitionKey);

        groupCreateCommand.Item.GroupName = parameters.Request.GroupName;

        // save in data store
        var groupCreateResult = await groupCreateCommand.SaveAsync(requestContext, default);

        // return the group model
        return groupCreateResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromBody]
        public required CreateGroupRequest Request { get; init; }
    }
}
