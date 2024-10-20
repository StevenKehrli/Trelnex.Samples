using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Groups.Api.Objects;
using Trelnex.Groups.Client;

namespace Trelnex.Groups.Api.Endpoints;

internal static class GetGroupEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/groups/{groupId:guid}",
                HandleRequest)
            .RequirePermission<GroupsPermission.GroupsReadPolicy>()
            .Produces<GroupModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("GetGroup")
            .WithDescription("Gets the specified group")
            .WithTags("Groups");
    }

    public static async Task<GroupModel> HandleRequest(
        [FromServices] ICommandProvider<IGroup> groupProvider,
        [AsParameters] RequestParameters parameters)
    {
        // get the group dto from data store
        var groupReadResult = await groupProvider.ReadAsync(
            id: parameters.GroupId.ToString(),
            partitionKey: parameters.GroupId.ToString());

        // throw if not found
        if (groupReadResult is null)
        {
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        // return the group model
        return groupReadResult.Item.ConvertToModel();
    }

    public class RequestParameters
    {
        [FromRoute(Name = "groupId")]
        [SwaggerParameter(Description = "The specified group id", Required = true)]
        public required Guid GroupId { get; init; }
    }
}
