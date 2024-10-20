using Trelnex.Core.Api;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Cosmos;
using Trelnex.Core.Api.Swagger;
using Trelnex.Core.Data;
using Trelnex.Users.Api.Endpoints;
using Trelnex.Users.Api.Objects;

Application.Run(args, UsersApplication.Add, UsersApplication.Use);

internal static class UsersApplication
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
                options => options.AddUsersCommandProviders());
    }

    public static void Use(
        WebApplication app)
    {
        app
            .AddSwaggerToWebApplication()
            .UseEndpoints();
    }

    public static ICommandProviderOptions AddUsersCommandProviders(
        this ICommandProviderOptions options)
    {
        return options
            .Add<IUser, User>(
                typeName: "user",
                validator: User.Validator,
                commandOperations: CommandOperations.All);
    }

    private static IPermissionsBuilder AddPermissions(
        this IPermissionsBuilder permissionsBuilder,
        ILogger bootstrapLogger)
    {
        permissionsBuilder
            .AddPermissions<UsersPermission>(bootstrapLogger);

        return permissionsBuilder;
    }

    private static IEndpointRouteBuilder UseEndpoints(
        this IEndpointRouteBuilder erb)
    {
        CreateUserEndpoint.Map(erb);
        GetUserEndpoint.Map(erb);

        return erb;
    }
}
