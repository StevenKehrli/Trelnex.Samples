using Trelnex.Core.Api;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Client;
using Trelnex.Core.Api.Swagger;
using Trelnex.Core.Azure.CommandProviders;
using Trelnex.Core.Azure.Identity;
using Trelnex.Core.Data;
using Trelnex.Messages.Api.Endpoints;
using Trelnex.Messages.Api.Items;
using Trelnex.Users.Client;

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
            .AddAzureIdentity(
                configuration,
                bootstrapLogger)
            .AddCosmosCommandProviders(
                configuration,
                bootstrapLogger,
                options => options.AddMessagesCommandProviders())
            .AddClient(
                configuration,
                new UsersClientFactory());
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
            .Add<IMessageItem, MessageItem>(
                typeName: "message",
                validator: MessageItem.Validator,
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
