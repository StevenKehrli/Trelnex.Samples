using Trelnex.Core.Api;
using Trelnex.Core.Api.Authentication;
using Trelnex.Core.Api.Swagger;
using Trelnex.Core.Azure.Identity;
using Trelnex.Core.Data;
using Trelnex.Groups.Api.Endpoints;
using Trelnex.Groups.Api.Objects;

Application.Run(args, GroupsApplication.Add, GroupsApplication.Use);

internal static class GroupsApplication
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
                options => options.AddGroupsCommandProviders());
    }

    public static void Use(
        WebApplication app)
    {
        app
            .AddSwaggerToWebApplication()
            .UseEndpoints();
    }

    public static ICommandProviderOptions AddGroupsCommandProviders(
        this ICommandProviderOptions options)
    {
        return options
            .Add<IGroup, Group>(
                typeName: "group",
                validator: Group.Validator,
                commandOperations: CommandOperations.All);
    }

    private static IPermissionsBuilder AddPermissions(
        this IPermissionsBuilder permissionsBuilder,
        ILogger bootstrapLogger)
    {
        permissionsBuilder
            .AddPermissions<GroupsPermission>(bootstrapLogger);

        return permissionsBuilder;
    }

    private static IEndpointRouteBuilder UseEndpoints(
        this IEndpointRouteBuilder erb)
    {
        CreateGroupEndpoint.Map(erb);
        GetGroupEndpoint.Map(erb);

        return erb;
    }
}
