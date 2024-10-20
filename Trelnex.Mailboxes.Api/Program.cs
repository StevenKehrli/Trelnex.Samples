using Trelnex.Core.Api;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Cosmos;
using Trelnex.Core.Api.Swagger;
using Trelnex.Core.Data;
using Trelnex.Groups.Client;
using Trelnex.Mailboxes.Api.Endpoints;
using Trelnex.Mailboxes.Api.Objects;
using Trelnex.Users.Client;

Application.Run(args, MailboxesApplication.Add, MailboxesApplication.Use);

internal static class MailboxesApplication
{
    public static void Add(
        IServiceCollection services,
        IConfiguration configuration,
        ILogger bootstrapLogger)
    {
        services
            .AddAuthentication(configuration)
            .AddPemissions(bootstrapLogger);

        services
            .AddSwaggerToServices()
            .AddCosmosCommandProviders(
                configuration,
                bootstrapLogger,
                options => options.AddMailboxesCommandProviders())
            .WithGroupsClient(
                configuration,
                bootstrapLogger)
            .WithUsersClient(
                configuration,
                bootstrapLogger);
    }

    public static void Use(
        WebApplication app)
    {
        app
            .AddSwaggerToWebApplication()
            .UseEndpoints();
    }

    public static ICommandProviderOptions AddMailboxesCommandProviders(
        this ICommandProviderOptions options)
    {
        return options
            .Add<IMailbox, Mailbox>(
                typeName: "mailbox",
                validator: Mailbox.Validator,
                commandOperations: CommandOperations.None)
            .Add<IGroupMailbox, GroupMailbox>(
                typeName: "group-mailbox",
                validator: GroupMailbox.Validator,
                commandOperations: CommandOperations.None)
            .Add<IUserMailbox, UserMailbox>(
                typeName: "user-mailbox",
                validator: UserMailbox.Validator,
                commandOperations: CommandOperations.None);
    }

    private static IPermissionsBuilder AddPemissions(
        this IPermissionsBuilder permissionsBuilder,
        ILogger bootstrapLogger)
    {
        permissionsBuilder
            .AddPermissions<MailboxesPermission>(bootstrapLogger);

        return permissionsBuilder;
    }

    private static IEndpointRouteBuilder UseEndpoints(
        this IEndpointRouteBuilder erb)
    {
        GetGroupMailboxEndpoint.Map(erb);
        GetMailboxEndpoint.Map(erb);
        GetUserMailboxEndpoint.Map(erb);

        return erb;
    }
}
