using Trelnex.Core.Api;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.CommandProviders;
using Trelnex.Core.Api.Swagger;
using Trelnex.Core.Data;
using Trelnex.Mailboxes.Client;
using Trelnex.Messages.Api.Endpoints;
using Trelnex.Messages.Api.Objects;

Application.Run(args, MessagesApplication.Add, MessagesApplication.Use);

internal static class MessagesApplication
{
    public static void Add(
        IServiceCollection services,
        IConfiguration configuration,
        ILogger bootstrapLogger)
    {
        services
            .AddAuthentication(configuration)
            .AddPermissions(bootstrapLogger);

        services
            .AddSwaggerToServices()
            .AddCosmosCommandProviders(
                configuration,
                bootstrapLogger,
                options => options.AddMessagesCommandProviders())
            .WithMailboxesClient(
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

    public static ICommandProviderOptions AddMessagesCommandProviders(
        this ICommandProviderOptions options)
    {
        return options
            .Add<IMessage, Message>(
                typeName: "message",
                validator: Message.Validator,
                commandOperations: CommandOperations.All);
    }

    private static IPermissionsBuilder AddPermissions(
        this IPermissionsBuilder permissionsBuilder,
        ILogger bootstrapLogger)
    {
        permissionsBuilder
            .AddPermissions<MessagesPermission>(bootstrapLogger);

        return permissionsBuilder;
    }

    private static IEndpointRouteBuilder UseEndpoints(
        this IEndpointRouteBuilder erb)
    {
        CreateMessageEndpoint.Map(erb);
        DeleteMessageEndpoint.Map(erb);
        GetMessageEndpoint.Map(erb);
        GetMessagesEndpoint.Map(erb);
        UpdateMessageEndpoint.Map(erb);

        return erb;
    }
}
