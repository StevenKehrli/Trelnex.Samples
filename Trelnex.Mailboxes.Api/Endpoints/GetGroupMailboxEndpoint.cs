using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Groups.Client;
using Trelnex.Mailboxes.Api.Objects;
using Trelnex.Mailboxes.Client;

namespace Trelnex.Mailboxes.Api.Endpoints;

internal static class GetGroupMailboxEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/groups/{groupId:guid}/mailbox",
                HandleRequest)
            .RequirePermission<MailboxesPermission.MailboxesReadPolicy>()
            .Produces<MailboxModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("GetGroupMailbox")
            .WithDescription("Gets the mailbox for the specified group.")
            .WithTags("Mailboxes")
            .ValidateGroup(efiContext =>
            {
                // get our request parameters
                var groupsClient = efiContext.Arguments.OfType<IGroupsClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (groupsClient, requestParameters.GroupId);
            });
    }

    public static async Task<MailboxModel> HandleRequest(
        [FromServices] IGroupsClient groupsClient,
        [FromServices] ICommandProvider<IMailbox> mailboxProvider,
        [FromServices] ICommandProvider<IGroupMailbox> groupMailboxProvider,
        [FromServices] IRequestContext requestContext,
        [AsParameters] RequestParameters parameters)
    {
        // get the mailbox owner dto from data store
        var groupMailboxReadResult = await GetGroupMailbox(
            mailboxProvider: mailboxProvider,
            groupMailboxProvider: groupMailboxProvider,
            requestContext: requestContext,
            groupId: parameters.GroupId);

        // if not found, there was a problem creating the dtos
        if (groupMailboxReadResult is null) throw new HttpStatusCodeException(HttpStatusCode.FailedDependency);

        // return the mailbox
        return groupMailboxReadResult.ConvertToModel();
    }

    private static async Task<IGroupMailbox?> GetGroupMailbox(
        ICommandProvider<IMailbox> mailboxProvider,
        ICommandProvider<IGroupMailbox> groupMailboxProvider,
        IRequestContext requestContext,
        Guid groupId)
    {
        // get the mailbox owner dto from data store
        var groupMailboxReadResult = await groupMailboxProvider.ReadAsync(
            id: groupId.ToString(),
            partitionKey: groupId.ToString());

        // if exists, return
        if (groupMailboxReadResult is not null) return groupMailboxReadResult.Item;

        // create the dtos
        var mailboxId = Guid.NewGuid();

        // create the mailbox dto
        var mailboxCreateCommand = mailboxProvider.Create(
            id: mailboxId.ToString(),
            partitionKey: mailboxId.ToString());

        mailboxCreateCommand.Item.OwnerId = groupId;
        mailboxCreateCommand.Item.OwnerType = MailboxOwnerType.Group;

        // create the group mailbox dto
        var groupMailboxCreateCommand = groupMailboxProvider.Create(
            id: groupId.ToString(),
            partitionKey: groupId.ToString());

        groupMailboxCreateCommand.Item.OwnerType = MailboxOwnerType.Group;
        groupMailboxCreateCommand.Item.MailboxId = mailboxId;

        try
        {
            // save the group mailbox to data store
            var groupMailboxCreateResult = await groupMailboxCreateCommand.SaveAsync(requestContext, default);

            // save the mailbox to data store
            await mailboxCreateCommand.SaveAsync(requestContext, default);

            return groupMailboxCreateResult.Item;
        }
        catch (CommandException)
        {
            // failed - likely a concurrency error - group mailbox now exists
            groupMailboxReadResult = await groupMailboxProvider.ReadAsync(
                id: groupId.ToString(),
                partitionKey: groupId.ToString());

            return groupMailboxReadResult?.Item;
        }
    }

    public class RequestParameters
    {
        [FromRoute(Name = "groupId")]
        [SwaggerParameter(Description = "The specified group id.", Required = true)]
        public required Guid GroupId { get; init; }
    }
}
