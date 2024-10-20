using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Trelnex.Core;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Responses;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Api.Objects;
using Trelnex.Mailboxes.Client;
using Trelnex.Users.Client;

namespace Trelnex.Mailboxes.Api.Endpoints;

internal static class GetUserMailboxEndpoint
{
    public static void Map(
        IEndpointRouteBuilder erb)
    {
        erb.MapGet(
                "/users/{userId:guid}/mailbox",
                HandleRequest)
            .RequirePermission<MailboxesPermission.MailboxesReadPolicy>()
            .Produces<MailboxModel>()
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status401Unauthorized)
            .Produces<HttpStatusCodeResponse>(StatusCodes.Status403Forbidden)
            .WithName("GetUserMailbox")
            .WithDescription("Gets the mailbox for the specified user.")
            .WithTags("Mailboxes")
            .ValidateUser(efiContext =>
            {
                // get our request parameters
                var usersClient = efiContext.Arguments.OfType<IUsersClient>().First();
                var requestParameters = efiContext.Arguments.OfType<RequestParameters>().First();

                return (usersClient, requestParameters.UserId);
            });
    }

    public static async Task<MailboxModel> HandleRequest(
        [FromServices] IUsersClient usersClient,
        [FromServices] ICommandProvider<IMailbox> mailboxProvider,
        [FromServices] ICommandProvider<IUserMailbox> userMailboxProvider,
        [FromServices] IRequestContext requestContext,
        [AsParameters] RequestParameters parameters)
    {
        // get the mailbox owner dto from data store
        var userMailboxReadResult = await GetUserMailbox(
            mailboxProvider: mailboxProvider,
            userMailboxProvider: userMailboxProvider,
            requestContext: requestContext,
            userId: parameters.UserId);

        // if not found, there was a problem creating the dtos
        if (userMailboxReadResult is null) throw new HttpStatusCodeException(HttpStatusCode.FailedDependency);

        // return the mailbox
        return userMailboxReadResult.ConvertToModel();
    }

    private static async Task<IUserMailbox?> GetUserMailbox(
        ICommandProvider<IMailbox> mailboxProvider,
        ICommandProvider<IUserMailbox> userMailboxProvider,
        IRequestContext requestContext,
        Guid userId)
    {
        // get the mailbox owner dto from data store
        var userMailboxReadResult = await userMailboxProvider.ReadAsync(
            id: userId.ToString(),
            partitionKey: userId.ToString());

        // if exists, return
        if (userMailboxReadResult is not null) return userMailboxReadResult.Item;

        // create the dtos
        var mailboxId = Guid.NewGuid();

        // create the mailbox dto
        var mailboxCreateCommand = mailboxProvider.Create(
            id: mailboxId.ToString(),
            partitionKey: mailboxId.ToString());

        mailboxCreateCommand.Item.OwnerId = userId;
        mailboxCreateCommand.Item.OwnerType = MailboxOwnerType.User;

        // create the user mailbox dto
        var userMailboxCreateCommand = userMailboxProvider.Create(
            id: userId.ToString(),
            partitionKey: userId.ToString());

        userMailboxCreateCommand.Item.OwnerType = MailboxOwnerType.User;
        userMailboxCreateCommand.Item.MailboxId = mailboxId;

        try
        {
            // save the user mailbox to data store
            var userMailboxCreateResult = await userMailboxCreateCommand.SaveAsync(requestContext, default);

            // save the mailbox to data store
            await mailboxCreateCommand.SaveAsync(requestContext, default);

            return userMailboxCreateResult.Item;
        }
        catch (CommandException)
        {
            // failed - concurrency error - try again
            userMailboxReadResult = await userMailboxProvider.ReadAsync(
                id: userId.ToString(),
                partitionKey: userId.ToString());

            return userMailboxReadResult?.Item;
        }
    }

    public class RequestParameters
    {
        [FromRoute(Name = "userId")]
        [SwaggerParameter(Description = "The specified user id.", Required = true)]
        public required Guid UserId { get; init; }
    }
}
